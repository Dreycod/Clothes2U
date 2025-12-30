using Shared.DTO;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
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
    private ListableViewModel<CategorieDTO> VM_Categorie;
    private WritableService<SousCategoriePostDTO> SousCategorieService;

    public event Action? OnStateChange;
    public CommercialSousCategoriesViewModel(ListableViewModel<CategorieDTO> categorieService, WritableService<SousCategoriePostDTO> sousCategorieService)
    {
        VM_Categorie = categorieService;
        SousCategorieService = sousCategorieService;
    }
    public async Task LoadAsync()
    {
        await VM_Categorie.LoadAsync();
        LoadAllSubcategories();
    }

    public void LoadAllSubcategories()
    {
        allSubcategories.Clear();
        if (VM_Categorie.Items != null)
        {
            foreach (var category in VM_Categorie.Items)
            {
                if (category.SousCategories != null)
                {
                    foreach (var subcat in category.SousCategories)
                    {
                        allSubcategories.Add((subcat, category));
                    }
                }
            }
        }
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
                await SousCategorieService.UpdateAsync(sousCategoriePost);
                successMessage = "Sous-catégorie modifiée avec succès";
            }
            else
            {
                await SousCategorieService.AddAsync(sousCategoriePost);
                successMessage = "Sous-catégorie ajoutée avec succès";
            }

            CloseModal();
            await VM_Categorie.LoadAsync();
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
            await SousCategorieService.DeleteAsync(currentSousCategorie.SousCategorieId);
            successMessage = $"Sous-catégorie {currentSousCategorie.LibelleSousCategorie} supprimée avec succès";
            CloseDeleteModal();
            await VM_Categorie.LoadAsync();
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

