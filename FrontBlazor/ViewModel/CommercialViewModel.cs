using Shared.DTO;
using Shared.DTO.Marque;
using Shared.DTO.Categorie;
using Shared.DTO.Couleur;
using Shared.DTO.Taille;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using Microsoft.AspNetCore.Components;

namespace FrontBlazor.ViewModel;

public class CommercialViewModel
{
    NavigationManager _navigationManager;
    #region Attributes
    public int totalMarques;
    public int totalCategories;
    public int totalSousCategories;
    public int totalCouleurs;
    public int totalTailles;
    #endregion

    #region ViewModels
    ListableViewModel<MarqueDTO> VM_Marque;
    ListableViewModel<CategorieDetailDTO> VM_Categorie;
    ListableViewModel<CouleurDTO> VM_Couleurs;
    ListableViewModel<TailleDTO> VM_Tailles;
    #endregion
    public Action? OnStateChange;
    public CommercialViewModel(NavigationManager navigationManager, ListableViewModel<CouleurDTO> vM_Couleurs, ListableViewModel<MarqueDTO> vM_Marque, ListableViewModel<CategorieDetailDTO> vM_Categorie, ListableViewModel<TailleDTO> vM_Tailles)
    {
        _navigationManager = navigationManager;
        VM_Couleurs = vM_Couleurs;
        VM_Marque = vM_Marque;
        VM_Categorie = vM_Categorie;
        VM_Tailles = vM_Tailles;
    }

    public async Task LoadAsync()
    {
        await VM_Couleurs.LoadAsync();
        await VM_Marque.LoadAsync();
        await VM_Categorie.LoadAsync();
        await VM_Tailles.LoadAsync();
        CountItems();
        OnStateChange?.Invoke();

    }
    public void CountItems()
    {
        totalCouleurs = VM_Couleurs.Items.Count();
        totalMarques = VM_Marque.Items.Count();
        totalCategories = VM_Categorie.Items.Count();
        totalSousCategories = VM_Categorie.Items.SelectMany(c => c.SousCategories).Count();
        totalTailles = VM_Tailles.Items.Count();
    }
    public void GoToPage(string page)
    {
        _navigationManager.NavigateTo(page);
    }

}
