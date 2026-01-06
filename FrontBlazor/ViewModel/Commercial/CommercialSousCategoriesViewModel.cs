using Shared.DTO;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Shared.DTO.Categorie;
using Shared.DTO.SousCategorie;

namespace FrontBlazor.ViewModel;
public class CommercialSousCategoriesViewModel
{
    public bool showModal = false;
    public bool showDeleteModal = false;
    public bool isEditing = false;
    public SousCategorieDTO currentSousCategorie = new SousCategorieDTO();
    public int selectedCategorieId = 0;
    public string successMessage = string.Empty;
    public string errorMessage = string.Empty;

    public List<(SousCategorieDTO Subcategory, CategorieDTO ParentCategory)> allSubcategories = new();
    private ICaracteristiqueService<CategorieDTO> _categorieService;
    private WritableService<SousCategoriePostDTO> _sousCategorieService;
    
    public List<CategorieDTO> Categories { get; set; }
    public bool IsLoading { get; set; }

    public event Action? OnStateChange;
    public CommercialSousCategoriesViewModel(ICaracteristiqueService<CategorieDTO> categorieService, WritableService<SousCategoriePostDTO> sousCategorieService)
    {
       _categorieService = categorieService;
       _sousCategorieService = sousCategorieService;
    }
    public async Task LoadAsync()
    {
        IsLoading = true;
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
        currentSousCategorie = new SousCategorieDTO();
        selectedCategorieId = 0;
        showModal = true;
    }

    public void ShowEditModal(SousCategorieDTO sousCategorie, CategorieDTO parentCategory)
    {
        isEditing = true;
        currentSousCategorie = new SousCategorieDTO
        {
            SousCategorieId = sousCategorie.SousCategorieId,
            LibelleSousCategorie = sousCategorie.LibelleSousCategorie,
            Categorie = parentCategory.LibelleCategorie
        };
        selectedCategorieId = parentCategory.IdCategorie;
        showModal = true;
    }

    public void ShowDeleteModal(SousCategorieDTO sousCategorie)
    {
        currentSousCategorie = sousCategorie;
        showDeleteModal = true;
    }

    public void CloseModal()
    {
        showModal = false;
        currentSousCategorie = new SousCategorieDTO();
        selectedCategorieId = 0;
        errorMessage = string.Empty;
    }

    public void CloseDeleteModal()
    {
        showDeleteModal = false;
        currentSousCategorie = new SousCategorieDTO();
    }

    public async Task SaveSousCategorie()
    {
        if (string.IsNullOrWhiteSpace(currentSousCategorie.LibelleSousCategorie))
        {
            errorMessage = "Le nom de la sous-catégorie est requis";
            return;
        }

        if (selectedCategorieId == 0)
        {
            errorMessage = "Veuillez sélectionner une catégorie parente";
            return;
        }

        try
        {
            SousCategoriePostDTO sousCategoriePost = new SousCategoriePostDTO
            {
                SousCategorieId = currentSousCategorie.SousCategorieId,
                LibelleSousCategorie = currentSousCategorie.LibelleSousCategorie,
                CategorieId = selectedCategorieId
            };

            if (isEditing)
            {
                await _sousCategorieService.UpdateAsync(sousCategoriePost);
                successMessage = "Sous-catégorie modifiée avec succès";
            }
            else
            {
                await _sousCategorieService.AddAsync(sousCategoriePost);
                successMessage = "Sous-catégorie ajoutée avec succès";
            }

            CloseModal();
            Categories = await _categorieService.GetAllAsync();
            LoadAllSubcategories();
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

    public async Task DeleteSousCategorie()
    {
        try
        {
            await _sousCategorieService.DeleteAsync(currentSousCategorie.SousCategorieId);
            successMessage = $"Sous-catégorie {currentSousCategorie.LibelleSousCategorie} supprimée avec succès";
            CloseDeleteModal();
            Categories = await _categorieService.GetAllAsync();
            LoadAllSubcategories();
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
}

