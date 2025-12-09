using FrontBlazor.Models;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.ViewModel;
public class CommercialCategoriesViewModel
{
    public bool showModal = false;
    public bool showDeleteModal = false;
    public bool isEditing = false;
    public Categorie currentCategorie = new Categorie();
    public string successMessage = string.Empty;
    public string errorMessage = string.Empty;

    private ListableViewModel<Categorie> VM_Categorie;
    public event Action? OnStateChange;

    public CommercialCategoriesViewModel(ListableViewModel<Categorie> categorieService)
    {
        VM_Categorie = categorieService;
    }
    public async Task LoadAsync()
    {
        await VM_Categorie.LoadAsync();
    }

    public void ShowAddModal()
    {
        isEditing = false;
        currentCategorie = new Categorie();
        showModal = true;
    }

    public void ShowEditModal(Categorie categorie)
    {
        isEditing = true;
        currentCategorie = new Categorie
        {
            IdCategorie = categorie.IdCategorie,
            LibelleCategorie = categorie.LibelleCategorie,
            SousCategories = categorie.SousCategories
        };
        showModal = true;
    }

    public void ShowDeleteModal(Categorie categorie)
    {
        currentCategorie = categorie;
        showDeleteModal = true;
    }

    public void CloseModal()
    {
        showModal = false;
        currentCategorie = new Categorie();
        errorMessage = string.Empty;
    }

    public void CloseDeleteModal()
    {
        showDeleteModal = false;
        currentCategorie = new Categorie();
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
                // TODO: Update categorie via API
                successMessage = "Catégorie modifiée avec succès";
            }
            else
            {
                // TODO: Create categorie via API
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
            // TODO: Delete categorie via API
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

    public int GetArticleCount(int categorieId)
    {
        // TODO: Get actual article count from API
        return new Random(categorieId).Next(50, 200);
    }
}

