using Shared.DTO;
using Shared.DTO.Marque;
using Shared.DTO.Categorie;
using Shared.DTO.Couleur;
using Shared.DTO.Taille;
using FrontBlazor.Services;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using FrontBlazor.ViewModel.Commercial;

namespace FrontBlazor.ViewModel;

public class CommercialViewModel: BaseCommercialViewModel
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
    private readonly ICaracteristiqueService<MarqueDTO> _marqueService;
    private readonly ICategorieService<CategorieDTO> _categorieService;
    private readonly ICaracteristiqueService<CouleurDTO> _couleurService;
    private readonly ICaracteristiqueService<TailleDTO> _tailleService;
    
    public List<CouleurDTO>  Couleurs { get; set; }
    public List<MarqueDTO> Marques { get; set; }
    public List<CategorieDTO> Categories { get; set; }
    public List<TailleDTO> Tailles { get; set; }
    #endregion
    public Action? OnStateChange;
    public CommercialViewModel(NavigationManager navigationManager, ICaracteristiqueService<CouleurDTO> couleurService, ICaracteristiqueService<MarqueDTO> marqueService, ICategorieService<CategorieDTO> categorieService, ICaracteristiqueService<TailleDTO> tailleService, IAuthService authService, NavigationManager nav) : base(authService, nav)
    {
        _navigationManager = navigationManager;
        _couleurService =  couleurService;
        _marqueService = marqueService;
        _categorieService = categorieService;
        _tailleService = tailleService;
        
    }

    public async Task LoadAsync()
    {
        await base.LoadAsync();
        Tailles = await _tailleService.GetAllAsync();
        Couleurs = await _couleurService.GetAllAsync();
        Marques = await _marqueService.GetAllAsync();
        Categories = await _categorieService.GetAllCategories();
        CountItems();
        OnStateChange?.Invoke();

    }
    public void CountItems()
    {
        totalCouleurs = Couleurs.Count();
        totalMarques = Marques.Count();
        totalCategories = Categories.Count();
        totalSousCategories = Categories.SelectMany(c => c.SousCategories).Count();
        totalTailles = Tailles.Count();
    }
    public void GoToPage(string page)
    {
        _navigationManager.NavigateTo(page);
    }

}
