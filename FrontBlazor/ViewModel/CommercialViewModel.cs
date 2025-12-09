using FrontBlazor.Models;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using Microsoft.AspNetCore.Components;

namespace FrontBlazor.ViewModel;

public class CommercialViewModel
{
    NavigationManager _navigationManager;
    #region Attributes
    public int totalMarques = 45;
    public int totalCategories = 12;
    public int totalSousCategories = 34;
    public int totalCouleurs = 28;
    public int totalTailles = 15;
    #endregion
    public CommercialViewModel(NavigationManager navigationManager)
    {
        _navigationManager = navigationManager;
    }

    public void GoToPage(string page)
    {
        _navigationManager.NavigateTo(page);
    }
    public async Task LoadAsync()
    {
        // TODO: Load actual statistics from database
        // await LoadStatistics();
    }
}
