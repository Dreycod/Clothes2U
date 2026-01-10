using FrontBlazor.Services;
using FrontBlazor.Services.Interfaces;
using FrontBlazor.ViewModel.Commercial;
using Microsoft.AspNetCore.Components;
using Shared.DTO;
using Shared.DTO.Categorie;
using Shared.DTO.Mesures;
using Shared.DTO.SousCategorie;
using Shared.DTO.Taille;

namespace FrontBlazor.ViewModel;

public class CommercialTaillesViewModel: BaseCommercialViewModel
{
    public bool showModal = false;
    public bool showDeleteModal = false;
    public bool isEditing = false;
    public TailleDTO currentTaille = new TailleDTO();
    public List<int> selectedSousCategoriesIds { get; set; }
    public string successMessage = string.Empty;
    public string errorMessage = string.Empty;
    
    public List<TailleDTO> Tailles { get; set; }
    public List<CategorieDTO> Categories { get; set; }
    public List<MesureDTO> mesuresToAdd { get; set; } = new List<MesureDTO>();
    public List<(SousCategorieDTO Subcategory, CategorieDTO ParentCategory)> allSubcategories = new();
    private readonly ICaracteristiqueService<CategorieDTO> _categorieService;
    private readonly ITailleService _tailleService;


    public bool IsLoading { get; set; }

    public event Action? OnStateChange;


    public CommercialTaillesViewModel(ITailleService tailleService, ICaracteristiqueService<CategorieDTO> categorieService, IAuthService authService, NavigationManager nav) : base(authService, nav)
    {
        _tailleService =  tailleService;
        _categorieService = categorieService;
    }
    public async Task LoadAsync()
    {
        IsLoading = true;
        await base.LoadAsync();
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
        mesuresToAdd.Clear();
        showModal = true;
    }

    public void ShowEditModal(TailleDTO taille)
    {
        isEditing = true;
        selectedSousCategoriesIds = taille.Mesures.Select(m => m.SousCategorieId).ToList();

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
        selectedSousCategoriesIds = new List<int>();
        errorMessage = string.Empty;
        mesuresToAdd.Clear();
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
            

            if (isEditing)
            {
                for (int i = 0; i < selectedSousCategoriesIds.Count; i++)
                {
                    MesureDTO newMesure = new MesureDTO
                    {
                        SousCategorieId = selectedSousCategoriesIds[i],
                        TailleId = currentTaille.TailleId
                    };
                    mesuresToAdd.Add(newMesure);
                }


                await _tailleService.PutTailleMesuresAsync(currentTaille.TailleId, mesuresToAdd);
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

