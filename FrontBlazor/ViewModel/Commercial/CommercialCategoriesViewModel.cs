using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using Shared.DTO;
using Shared.DTO.Categorie;

namespace FrontBlazor.ViewModel;
public class CommercialCategoriesViewModel
{
    public bool showModal = false;
    public bool showDeleteModal = false;
    public bool isEditing = false;
    public CategorieDTO currentCategorie = new CategorieDTO();
    public string successMessage = string.Empty;
    public string errorMessage = string.Empty;

    private ListableViewModel<CategorieDTO> VM_Categorie;
    private WritableService<CategorieDTO> CategorieService;

    public event Action? OnStateChange;

    public CommercialCategoriesViewModel(ListableViewModel<CategorieDTO> _vmCategorie, WritableService<CategorieDTO> _categorieService)
    {
        VM_Categorie = _vmCategorie;
        CategorieService = _categorieService;
    }
    public async Task LoadAsync()
    {
        await VM_Categorie.LoadWithDetailsAsync();
    }

    public void ShowAddModal()
    {
        isEditing = false;
        currentCategorie = new CategorieDTO();
        showModal = true;
    }

    public void ShowEditModal(CategorieDTO categorie)
    {
        isEditing = true;
        currentCategorie = new CategorieDTO
        {
            IdCategorie = categorie.IdCategorie,
            LibelleCategorie = categorie.LibelleCategorie,
            SousCategories = categorie.SousCategories
        };
        showModal = true;
    }

    public void ShowDeleteModal(CategorieDTO categorie)
    {
        currentCategorie = categorie;
        showDeleteModal = true;
    }

    public void CloseModal()
    {
        showModal = false;
        currentCategorie = new CategorieDTO();
        errorMessage = string.Empty;
    }

    public void CloseDeleteModal()
    {
        showDeleteModal = false;
        currentCategorie = new CategorieDTO();
    }

    public async Task SaveCategorie()
    {
        if (string.IsNullOrWhiteSpace(currentCategorie.LibelleCategorie))
        {
            errorMessage = "Le nom de la catégorie est requis";
            return;
        }

        try
        {
            if (isEditing)
            {
                await CategorieService.UpdateAsync(currentCategorie);
                successMessage = "Catégorie modifiée avec succès";
            }
            else
            {
                await CategorieService.AddAsync(currentCategorie);
                successMessage = "Catégorie ajoutée avec succès";
            }

            CloseModal();
            await VM_Categorie.LoadAsync();
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

    public async Task DeleteCategorie()
    {
        try
        {
            await CategorieService.DeleteAsync(currentCategorie.IdCategorie);
            successMessage = $"Catégorie {currentCategorie.LibelleCategorie} supprimée avec succès";
            CloseDeleteModal();
            await VM_Categorie.LoadAsync();
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

