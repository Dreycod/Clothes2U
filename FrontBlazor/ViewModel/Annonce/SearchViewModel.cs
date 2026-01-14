using FrontBlazor.Services;
using FrontBlazor.Services.Interfaces;
using FrontBlazor.ViewModel.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Shared.DTO;
using Shared.DTO.Annonce;
using Shared.DTO.Categorie;
using Shared.DTO.Couleur;
using Shared.DTO.EtatArticle;
using Shared.DTO.Favoris;
using Shared.DTO.Marque;
using Shared.DTO.SousCategorie;
using Shared.DTO.Taille;
using Shared.DTO.Utilisateur;
using System.Collections.ObjectModel;
using System.Data;

namespace FrontBlazor.ViewModel
{
    public class SearchAnnonceViewModel : ClientBaseViewModel
    {
        private readonly IAnnonceService _annonceService;
        private readonly IFavorisService<FavorisDTO> _favorisService;
        private readonly INotificationService _notificationPopUpService;
        private CancellationTokenSource _searchCts;
        private CancellationTokenSource _filterCts;

        public event Action? OnStateChange;

        #region Lists de champs
        public List<CategorieDTO> Categories { get; set; }
        public List<GenreDTO> Genres { get; set; }
        public List<MarqueDTO> Marques { get; set; }
        public List<TailleDTO> Tailles { get; set; }
        public List<EtatArticleDTO> Etats { get; set; }
        public List<CouleurDTO> Couleurs { get; set; } = new();
        public NavigationManager NavigationManager { get; set; }
        public LoginViewModel VM_Login { get; set; }
        #endregion

        #region services

        private readonly ICaracteristiqueService<CategorieDTO> _categorieService;
        private readonly IMarqueService _marqueService;
        private readonly ICaracteristiqueService<EtatArticleDTO> _etatService;
        private readonly ICaracteristiqueService<GenreDTO> _genreService;
        private readonly ICaracteristiqueService<TailleDTO> _tailleService;
        private readonly ICouleurService<CouleurDTO> _couleurService;
        #endregion

        #region Properties
        public List<AnnonceDTO> Annonces { get; set; } = new();
        public bool IsLoading { get; set; } = false;
        public string? ErrorMessage { get; set; }
        public string? Query { get; set; }

        public List<string> SelectedMarques { get; set; } = new();
        public List<string> SelectedGenres { get; set; } = new();
        public List<string> SelectedEtats { get; set; } = new();
        public HashSet<int> ExpandedCategories { get; set; } = new();
        public string marqueSearch { get; set; }
        public bool IsGenreExpanded { get; set; } = false;
        public bool IsCategoryExpanded { get; set; } = false;
        public bool IsSubCategoryExpanded { get; set; } = false;
        public bool IsSizeExpanded { get; set; } = false;
        public bool IsMarqueExpanded { get; set; } = false;
        public bool IsCouleurExpanded { get; set; } = false;
        public bool IsPriceExpanded { get; set; } = false;
        public bool IsEtatExpanded { get; set; } = false;
        public List<string> SelectedCouleurs { get; set; } = new();

        public int SliderMax { get; set; } = 500;
        public int SelectedMaxPrice { get; set; } = 250;
        public int SelectedMinPrice { get; set; } = 0;

        public int CurrentPage { get; set; } = 1;
        public int ItemsPerPage { get; set; } = 32;
        public int TotalItems { get; set; } = 0;
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
        #endregion

        public SearchAnnonceViewModel(
            IAnnonceService annonceService,
            IAuthService authService,
            IFavorisService<FavorisDTO> favorisService,
            ICaracteristiqueService<CategorieDTO> categorieService,
            IMarqueService marqueService,
            ICaracteristiqueService<EtatArticleDTO> etatService,
            ICaracteristiqueService<GenreDTO> genreService,
            ICaracteristiqueService<TailleDTO> tailleService,
            ICouleurService<CouleurDTO> couleurService,
            NavigationManager navManager,
            INotificationService notificationPopUpService,
            NavigationManager navigationManager,
            INotificationService notificationService,
            ISignalRService notificationHubService,
            LoginViewModel vM_Login)
        : base(navigationManager, authService, notificationHubService,notificationService)
        {
            _annonceService = annonceService;
            _favorisService = favorisService;

            _categorieService = categorieService;
            _marqueService = marqueService;
            _etatService = etatService;
            _genreService = genreService;
            _tailleService = tailleService;
            _couleurService = couleurService;
            NavigationManager = navManager;
            _notificationPopUpService = notificationPopUpService;
            VM_Login = vM_Login;
        }

        private void NotifyStateChanged() => OnStateChange?.Invoke();

        public async Task InitializeFromUrlAsync()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            string? query = null;
            string? genre = null;

            var queryParams = QueryHelpers.ParseQuery(uri.Query);

            if (queryParams.TryGetValue("q", out var q))
                query = q;

            if (queryParams.TryGetValue("genre", out var g))
                genre = g;

            await InitializeAsync(query, genre);
        }

        public async Task InitializeAsync(string? queryFromUrl, string? genreFromUrl = null)
        {
            Query = queryFromUrl;

            if (!string.IsNullOrEmpty(genreFromUrl))
            {
                SelectedGenres.Add(genreFromUrl);
            }
            await LoadAsync();
        }

        private async Task LoadAsync()
        {
            IsLoading = true;
            await base.LoadAsync();
            NotifyStateChanged();
            Categories = await _categorieService.GetAllAsync();
            Genres = await _genreService.GetAllAsync();
            Tailles = await _tailleService.GetAllAsync();
            Etats = await _etatService.GetAllAsync();
            Marques = await _marqueService.GetAllAsync();
            Couleurs = await _couleurService.GetAllAsync();
            await ApplyFilters();

            IsLoading = false;
            NotifyStateChanged();
        }

        #region Filter Actions
        public CategorieDTO? SelectedCategory { get; set; }

        public async Task ToggleCategory(CategorieDTO categorie)
        {
            if (SelectedCategory?.IdCategorie == categorie.IdCategorie)
            {
                SelectedCategory = null;
                SelectedSousCategory = null;
                TaillesDisponibles = null;
                SelectedTaille = null;
            }
            else
            {
                TaillesDisponibles = null;
                SelectedCategory = categorie;
            }

            await OnFilterChanged(); 
            NotifyStateChanged();
        }
        public SousCategorieDTO? SelectedSousCategory { get; set; }
        public List<TailleDTO>? TaillesDisponibles { get; set; } = null;

        public async Task ToggleSousCategory(SousCategorieDTO sousCategorie)
        {
            if (SelectedSousCategory?.SousCategorieId == sousCategorie.SousCategorieId) // ✅ Ajoute le ?
            {
                TaillesDisponibles = null;
                SelectedSousCategory = null;
                SelectedTaille = null;
            }
            else
            {
                SelectedSousCategory = sousCategorie;
                TaillesDisponibles = Tailles.Where(t => 
                    t.Mesures != null && 
                    t.Mesures.Any(m => m.SousCategorieId == SelectedSousCategory.SousCategorieId)
                ).ToList();
            }
    
            await OnFilterChanged(); 
            NotifyStateChanged();
        }

        public async Task ToggleFilter(List<string> list, string item)
        {
            if (list.Contains(item))
                list.Remove(item);
            else
                list.Add(item);

            await OnFilterChanged();
        }

        public async Task ResetFilters()
        {
            SelectedCategory = null;
            SelectedSousCategory = null;
            SelectedMarques.Clear();
            SelectedTaille = null;
            SelectedGenres.Clear();
            SelectedCouleurs.Clear();
            ExpandedCategories.Clear();
            CurrentPage = 1;
            SelectedMaxPrice = SliderMax / 2;

            await OnFilterChanged();
        }

        public async Task OnFilterChanged()
        {
            _filterCts?.Cancel();
            _filterCts = new CancellationTokenSource();
            var token = _filterCts.Token;

            try
            {
                await Task.Delay(300, token);
                await ApplyFilters();
            }
            catch (TaskCanceledException) { }
        }

        public async Task OnPriceChanged()
        {
            await OnFilterChanged();
        }
        public TailleDTO? SelectedTaille { get; set; }

        public async Task ToggleSelectedTaille(TailleDTO taille)
        {
            if (SelectedTaille?.TailleId == taille.TailleId)
            {
                SelectedTaille = null;
            }
            else
            {
                SelectedTaille = taille;
            }

            await OnFilterChanged(); 
            NotifyStateChanged();
        }

        private async Task ApplyFilters()
        {
            var filterRequest = new FilterDTO
            {
                MotCle = Query,
                Categories = SelectedCategory?.LibelleCategorie,
                SousCategories = SelectedSousCategory?.LibelleSousCategorie,
                Etats = SelectedEtats,
                Marques = SelectedMarques,
                Tailles = SelectedTaille?.Libelletaille,
                Genre = SelectedGenres,
                Couleurs = SelectedCouleurs,
                PrixMax = SelectedMaxPrice,
                PrixMin = SelectedMinPrice,
                SortOrder = SelectedSortOrder,
                SortBy = SelectedSortField
            };

            CurrentPage = 1;
            await LoadAnnonces(filterRequest);
        }

        public async Task OnSearchInput(string value)
        {
            Query = value;
            _searchCts?.Cancel();
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            try
            {
                await Task.Delay(500, token);
                await ApplyFilters();
                NotifyStateChanged();
            }
            catch (TaskCanceledException) { }
        }
        #endregion

        #region Annonce Actions
        private async Task LoadAnnonces(FilterDTO filterRequest)
        {
            await CountTotalItems(filterRequest);

            var result = await _annonceService.GetAnnonceByFilter(filterRequest, CurrentPage, ItemsPerPage);

            Annonces = result ?? new List<AnnonceDTO>();
            NotifyStateChanged();
        }

        private async Task CountTotalItems(FilterDTO filterRequest)
        {
            try
            {
                var result = await _annonceService.GetAnnonceByFilter(filterRequest, 1, 100000);
                TotalItems = result?.Count ?? 0;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erreur lors du comptage des annonces";
                Console.WriteLine($"Error: {ex.Message}");
                TotalItems = 0;
            }
        }

        public async Task ToggleFavorite(int annonceId)
        {
            if (utilisateur != null)
            {
                _nav.NavigateTo("/login");
                return;
            }

            var annonce = Annonces.FirstOrDefault(a => a.AnnonceId == annonceId);
            if (annonce == null) return;

            bool wasFavorite = annonce.IsLikedByCurrentUser;
            annonce.IsLikedByCurrentUser = !annonce.IsLikedByCurrentUser;

            try
            {
                if (!wasFavorite)
                    await _favorisService.AddFavoris(annonceId);
                else
                    await _favorisService.DeleteFavoris(annonceId);
            }
            catch (Exception ex)
            {
                annonce.IsLikedByCurrentUser = wasFavorite;
                Console.WriteLine($"Error toggling favorite: {ex.Message}");
            }

            NotifyStateChanged();
        }

        public void NavigateToProductDetail(int? annonceId)
        {
            if (annonceId.HasValue)
            {
                NavigationManager.NavigateTo($"/product/{annonceId}");
            }
        }
        #endregion

        #region Pagination Actions
        public async Task GoToPage(int page)
        {
            if (page < 1 || page > TotalPages)
                return;

            CurrentPage = page;

            var filterRequest = new FilterDTO
            {
                MotCle = Query,
                Categories = SelectedCategory?.LibelleCategorie,
                SousCategories = SelectedSousCategory?.LibelleSousCategorie,
                Marques = SelectedMarques,
                Tailles = SelectedTaille?.Libelletaille,
                Genre = SelectedGenres,
                PrixMax = SelectedMaxPrice,
                PrixMin = SelectedMinPrice
            };

            await LoadAnnonces(filterRequest);
        }

        public List<int> GetVisiblePages()
        {
            var pages = new List<int>();
            var maxVisible = 7;

            if (TotalPages <= maxVisible)
            {
                for (int i = 1; i <= TotalPages; i++)
                {
                    pages.Add(i);
                }
            }
            else
            {
                pages.Add(1);

                if (CurrentPage > 3)
                    pages.Add(-1);

                int start = Math.Max(2, CurrentPage - 1);
                int end = Math.Min(TotalPages - 1, CurrentPage + 1);

                for (int i = start; i <= end; i++)
                {
                    pages.Add(i);
                }

                if (CurrentPage < TotalPages - 2)
                    pages.Add(-1);

                pages.Add(TotalPages);
            }

            return pages;
        }

        public int GetStartItem() => (CurrentPage - 1) * ItemsPerPage + 1;
        public int GetEndItem() => Math.Min(CurrentPage * ItemsPerPage, TotalItems);
        #endregion


        public async Task GetFilteredMarques()
        {
            Marques = await _marqueService.SearchAsync(marqueSearch);
        }

        public SortField SelectedSortField { get; set; } = SortField.DateAnnonce;
        public SortOrder SelectedSortOrder { get; set; } = SortOrder.Descending;

        public async Task<bool> CheckIfOwnerAnnonce(AnnonceDTO annonce)
        {
            if (utilisateur == null)
                return false;

            return annonce.IdAuteur == utilisateur.UtilisateurId;
        }

        public void ToggleFilterExpand(string filterName)
        {
            var property = GetType().GetProperty(filterName);
            if (property != null && property.PropertyType == typeof(bool))
            {
                var currentValue = (bool)property.GetValue(this);
                property.SetValue(this, !currentValue);
                OnStateChange?.Invoke();
            }
        }
    }
}