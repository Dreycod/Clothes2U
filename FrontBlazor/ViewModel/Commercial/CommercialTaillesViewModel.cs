using Shared.DTO;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Shared.DTO.Taille;
using Shared.DTO.Categorie;

namespace FrontBlazor.ViewModel;

public class CommercialTaillesViewModel
{
    public bool showModal = false;
    public bool showDeleteModal = false;
    public bool isEditing = false;
    public TailleDTO currentTaille = new TailleDTO();
    public int selectedCategorieId = 0;
    public string successMessage = string.Empty;
    public string errorMessage = string.Empty;
    
    public List<TailleDTO> Tailles { get; set; }
    public List<CategorieDTO> Categories { get; set; }
    public bool IsLoading { get; set; }

    private ICaracteristiqueService<TailleDTO> _tailleService;
    private ICaracteristiqueService<CategorieDTO> _categorieService;
    public event Action? OnStateChange;

    public CommercialTaillesViewModel(ICaracteristiqueService<TailleDTO> tailleService, ICaracteristiqueService<CategorieDTO> categorieService)
    {
        _tailleService =  tailleService;
        _categorieService = categorieService;
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
        IsLoading = false;
    }

    public void ShowAddModal()
    {
        isEditing = false;
        currentTaille = new TailleDTO();
        selectedCategorieId = 0;
        showModal = true;
    }

    public void ShowEditModal(TailleDTO taille)
    {
        isEditing = true;
        currentTaille = new TailleDTO
        {
        //    TailleId = taille.TailleId,
        //    Libelletaille = taille.Libelletaille,
        //    CategorieId = taille.CategorieId
        };
        //selectedCategorieId = taille.CategorieId;
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
        selectedCategorieId = 0;
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

        if (selectedCategorieId == 0)
        {
            errorMessage = "Veuillez sélectionner une catégorie";
            return;
        }

        try
        {
            //currentTaille.CategorieId = selectedCategorieId;

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

            // Clear success message after 3 seconds
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
}

