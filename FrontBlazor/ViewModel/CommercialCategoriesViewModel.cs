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
    public CategorieDetailDTO currentCategorie = new CategorieDetailDTO();
    public string successMessage = string.Empty;
    public string errorMessage = string.Empty;

    private ListableViewModel<CategorieDetailDTO> VM_Categorie;
    public event Action? OnStateChange;

    public CommercialCategoriesViewModel(ListableViewModel<CategorieDetailDTO> categorieService)
    {
        VM_Categorie = categorieService;
    }
    public async Task LoadAsync()
    {
        await VM_Categorie.LoadWithDetailsAsync();
        Console.WriteLine("Categories loaded: " + VM_Categorie.Items.Count);
        // Print nombre produits de chaque 
        foreach (var categorie in VM_Categorie.Items)
        {
            Console.WriteLine($"Catégorie: {categorie.LibelleCategorie}, Nombre de sous-catégories: {categorie.SousCategories?.Count ?? 0}, Nombre de articles: {categorie.NombreProduits}");
        }
    }

    public void ShowAddModal()
    {
        isEditing = false;
        currentCategorie = new CategorieDetailDTO();
        showModal = true;
    }

    public void ShowEditModal(CategorieDetailDTO categorie)
    {
        isEditing = true;
        currentCategorie = new CategorieDetailDTO
        {
            IdCategorie = categorie.IdCategorie,
            LibelleCategorie = categorie.LibelleCategorie,
            SousCategories = categorie.SousCategories
        };
        showModal = true;
    }

    public void ShowDeleteModal(CategorieDetailDTO categorie)
    {
        currentCategorie = categorie;
        showDeleteModal = true;
    }

    public void CloseModal()
    {
        showModal = false;
        currentCategorie = new CategorieDetailDTO();
        errorMessage = string.Empty;
    }

    public void CloseDeleteModal()
    {
        showDeleteModal = false;
        currentCategorie = new CategorieDetailDTO();
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
}

