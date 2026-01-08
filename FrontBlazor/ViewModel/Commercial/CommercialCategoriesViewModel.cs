using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using FrontBlazor.ViewModel.Commercial;
using Microsoft.AspNetCore.Components;
using Shared.DTO;
using Shared.DTO.Categorie;
using Shared.DTO.Utilisateur;

namespace FrontBlazor.ViewModel;
public class CommercialCategoriesViewModel: BaseCommercialViewModel
{
    public bool showModal = false;
    public bool showDeleteModal = false;
    public bool isEditing = false;
    public CategorieDTO currentCategorie = new CategorieDTO();
    public string successMessage = string.Empty;
    public string errorMessage = string.Empty;
    public bool isCommercial = false;
    public List<CategorieDTO> Categories  { get; set; }
    private readonly ICaracteristiqueService<CategorieDTO> _categorieService;

    public event Action? OnStateChange;
    public bool IsLoading { get; set; }

    public CommercialCategoriesViewModel(ICaracteristiqueService<CategorieDTO> categorieService, IAuthService authService, NavigationManager nav) : base(authService, nav)
    {
        _categorieService = categorieService;
    }
    public async Task LoadAsync()
    {
        IsLoading = true;
        await base.LoadAsync();
        Categories = await _categorieService.GetAllAsync();

        if (Categories != null)
        {
            Categories = Categories
                .OrderBy(c => c.IdCategorie)
                .ToList();
        }
        IsLoading = false;
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
                await _categorieService.UpdateAsync(currentCategorie);
                successMessage = "Catégorie modifiée avec succès";
            }
            else
            {
                await _categorieService.AddAsync(currentCategorie);
                successMessage = "Catégorie ajoutée avec succès";
            }

            CloseModal();
            Categories = await _categorieService.GetAllAsync();
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
            await _categorieService.DeleteAsync(currentCategorie.IdCategorie);
            successMessage = $"Catégorie {currentCategorie.LibelleCategorie} supprimée avec succès";
            CloseDeleteModal();
            Categories = await _categorieService.GetAllAsync();
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

