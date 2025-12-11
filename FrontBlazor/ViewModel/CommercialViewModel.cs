using FrontBlazor.Models;
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
    ListableViewModel<Marque> VM_Marque;
    ListableViewModel<Categorie> VM_Categorie;
    ListableViewModel<Couleur> VM_Couleurs;
    ListableViewModel<Taille> VM_Tailles;
    #endregion
    public Action? OnStateChange;
    public CommercialViewModel(NavigationManager navigationManager, ListableViewModel<Couleur> vM_Couleurs, ListableViewModel<Marque> vM_Marque, ListableViewModel<Categorie> vM_Categorie, ListableViewModel<Taille> vM_Tailles)
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
