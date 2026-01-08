using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Shared.DTO;
using Shared.DTO.Categorie;
using Shared.DTO.Mesures;
using Shared.DTO.SousCategorie;
using Shared.DTO.Taille;

namespace FrontBlazor.ViewModel;

public class CommercialTaillesViewModel
{
    public bool showModal = false;
    public bool showDeleteModal = false;
    public bool isEditing = false;
    public TailleDTO currentTaille = new TailleDTO();
    public List<int> beforeChangeSousCategoriesIds { get; set; }
    public List<int> selectedSousCategoriesIds { get; set; }
    public string successMessage = string.Empty;
    public string errorMessage = string.Empty;
    
    public List<TailleDTO> Tailles { get; set; }
    public List<CategorieDTO> Categories { get; set; }
    public List<(SousCategorieDTO Subcategory, CategorieDTO ParentCategory)> allSubcategories = new();
    private readonly ICaracteristiqueService<CategorieDTO> _categorieService;
    private readonly ICaracteristiqueService<TailleDTO> _tailleService;
    private readonly ICaracteristiqueService<MesureDTO> _mesureService;

    public bool IsLoading { get; set; }

    public event Action? OnStateChange;


    public CommercialTaillesViewModel(ICaracteristiqueService<TailleDTO> tailleService, ICaracteristiqueService<CategorieDTO> categorieService, ICaracteristiqueService<MesureDTO> mesureService )
    {
        _tailleService =  tailleService;
        _categorieService = categorieService;
        _mesureService = mesureService;
    }
    public async Task LoadAsync()
    {
        IsLoading = true;
        Tailles = await _tailleService.GetAllAsync();
        if (Tailles != null)
        {
            Tailles = Tailles
                .OrderBy(c => c.TailleId)
                .ToList();
        }
        Categories = await _categorieService.GetAllAsync();
        LoadAllSubcategories();

        IsLoading = false;
    }

    public void LoadAllSubcategories()
    {
        allSubcategories.Clear();

        if (Categories == null)
            return;

        allSubcategories = Categories
            .Where(c => c.SousCategories != null)
            .SelectMany(c => c.SousCategories.Select(sc => (subcat: sc, category: c)))
            .OrderBy(x => x.subcat.SousCategorieId)
            .ToList();
    }

    public void ShowAddModal()
    {
        isEditing = false;
        currentTaille = new TailleDTO();
        selectedSousCategoriesIds = new List<int>();
        showModal = true;
    }

    public void ShowEditModal(TailleDTO taille)
    {
        isEditing = true;
        selectedSousCategoriesIds = taille.Mesures.Select(m => m.SousCategorieId).ToList();
        beforeChangeSousCategoriesIds = selectedSousCategoriesIds;

        currentTaille = new TailleDTO
        {
            TailleId = taille.TailleId,
            Libelletaille = taille.Libelletaille,
        };

        showModal = true;
    }

    public void ShowDeleteModal(TailleDTO taille)
    {
        currentTaille = taille;
        showDeleteModal = true;
    }

    public void CloseModal()
    {
        showModal = false;
        currentTaille = new TailleDTO();
        beforeChangeSousCategoriesIds = new List<int>();
        selectedSousCategoriesIds = new List<int>();
        errorMessage = string.Empty;
    }

    public void CloseDeleteModal()
    {
        showDeleteModal = false;
        currentTaille = new TailleDTO();
    }

    public async Task SaveTaille()
    {
        if (string.IsNullOrWhiteSpace(currentTaille.Libelletaille))
        {
            errorMessage = "Le libellé de la taille est requis";
            return;
        }

        if (selectedSousCategoriesIds.Count == 0)
        {
            errorMessage = "Veuillez sélectionner une catégorie";
            return;
        }

        try
        {
            var mesuresToAdd = selectedSousCategoriesIds.Except(beforeChangeSousCategoriesIds).ToList();
            var mesuresToRemove = beforeChangeSousCategoriesIds.Except(selectedSousCategoriesIds).ToList();
            Console.WriteLine("Mesures to add: " + string.Join(", ", mesuresToAdd));
            Console.WriteLine("Mesures to remove: " + string.Join(", ", mesuresToRemove));
            
            for (int i = 0; i < mesuresToAdd.Count; i++)
            {
                MesureDTO newMesure = new MesureDTO
                {
                    TailleId = currentTaille.TailleId,
                    SousCategorieId = mesuresToAdd[i]
                };
                await _mesureService.AddAsync(newMesure);
            }

            for (int i = 0; i < mesuresToRemove.Count; i++)
            {
                var mesureToDelete = currentTaille.Mesures.FirstOrDefault(m => m.SousCategorieId == mesuresToRemove[i]);
                if (mesureToDelete != null)
                {
                    await _mesureService.DeleteAsync(mesureToDelete.MesureId);
                }
            }

            if (isEditing)
            {
                await _tailleService.UpdateAsync(currentTaille);

                successMessage = "Taille modifiée avec succès";
            }
            else
            {
                await _tailleService.AddAsync(currentTaille);
                successMessage = "Taille ajoutée avec succès";
            }

            CloseModal();
            Tailles = await _tailleService.GetAllAsync();
            OnStateChange?.Invoke();

            await Task.Delay(3000);
            successMessage = string.Empty;
            OnStateChange?.Invoke();
        }
        catch (Exception ex)
        {
            errorMessage = $"Erreur: {ex.Message}";
        }
    }

    public async Task DeleteTaille()
    {
        try
        {
            await _tailleService.DeleteAsync(currentTaille.TailleId);
            successMessage = $"Taille {currentTaille.Libelletaille} supprimée avec succès";
            CloseDeleteModal();
            Tailles = await _tailleService.GetAllAsync();
            OnStateChange?.Invoke();

            // Clear success message after 3 seconds
            await Task.Delay(3000);
            successMessage = string.Empty;
            OnStateChange?.Invoke();

        }
        catch (Exception ex)
        {
            errorMessage = $"Erreur: {ex.Message}";
            CloseDeleteModal();
        }
    }

    public string GetCategoriesNumber(int idTaille)
    {
        int? categoryNumber = Tailles?.FirstOrDefault(c => c.TailleId == idTaille).Mesures.Count();
        if (categoryNumber == null)
            return "0";

        return categoryNumber.ToString();
    }

    public void ToggleCategory(ChangeEventArgs e, int categoryId)
    {
        bool isChecked = (bool)e.Value;

        if (isChecked && !selectedSousCategoriesIds.Contains(categoryId))
            selectedSousCategoriesIds.Add(categoryId);

        if (!isChecked && selectedSousCategoriesIds.Contains(categoryId))
            selectedSousCategoriesIds.Remove(categoryId);
    }
}

