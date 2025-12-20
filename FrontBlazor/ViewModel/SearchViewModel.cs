using System.Collections.ObjectModel;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Shared.DTO;
using Shared.DTO.Annonce;
using Shared.DTO.Categorie;
using Shared.DTO.EtatArticle;
using Shared.DTO.Favoris;
using Shared.DTO.Marque;
using Shared.DTO.Taille;

namespace FrontBlazor.ViewModel
{
    public class SearchAnnonceViewModel
    {
        private readonly IAnnonceService _annonceService;
        private readonly IFavorisService<FavorisDTO> _favorisService;
        private readonly INotificationService _notificationPopUpService;
        private CancellationTokenSource _searchCts;
        private CancellationTokenSource _filterCts;

        public event Action? OnStateChange;

        #region ViewModels
        public ListableViewModel<CategorieDTO> VM_Categorie { get; set; }
        public ListableViewModel<GenreDTO> VM_Genre { get; set; }
        public ListableViewModel<MarqueDTO> VM_Marque { get; set; }
        public ListableViewModel<TailleDTO> VM_Taille { get; set; }
        public ListableViewModel<EtatArticleDTO> VM_Etat { get; set; }
        public NavigationManager NavigationManager { get; set; }
        public LoginViewModel VM_Login { get; set; }
        #endregion

        #region Properties
        public List<AnnonceDTO> Annonces { get; set; } = new();
        public bool IsLoading { get; set; } = false;
        public string? ErrorMessage { get; set; }
        public string? Query { get; set; }
        
        public List<string> SelectedCategories { get; set; } = new();
        public List<string> SelectedSousCategories { get; set; } = new();
        public List<string> SelectedMarques { get; set; } = new();
        public List<string> SelectedTailles { get; set; } = new();
        public List<string> SelectedGenres { get; set; } = new();
        public List<string> SelectedEtats { get; set; } = new();
        public HashSet<int> ExpandedCategories { get; set; } = new();

        public int SliderMax { get; set; } = 500;
        public int SelectedPrice { get; set; } = 250;

        public int CurrentPage { get; set; } = 1;
        public int ItemsPerPage { get; set; } = 32;
        public int TotalItems { get; set; } = 0;
        public int TotalPages => (int)Math.Ceiling((double)TotalItems / ItemsPerPage);
        #endregion

        public SearchAnnonceViewModel(
            IAnnonceService annonceService, 
            IFavorisService<FavorisDTO> favorisService, 
            ListableViewModel<CategorieDTO> vM_Categorie, 
            ListableViewModel<MarqueDTO> vM_Marque, 
            ListableViewModel<EtatArticleDTO> vM_Etat,
            ListableViewModel<GenreDTO> vM_Genre,
            ListableViewModel<TailleDTO> vM_Taille, 
            NavigationManager navManager, 
            INotificationService notificationPopUpService,
            LoginViewModel vM_Login)
        {
            _annonceService = annonceService;
            _favorisService = favorisService;
            VM_Genre =  vM_Genre;
            VM_Categorie = vM_Categorie;
            VM_Marque = vM_Marque;
            VM_Taille = vM_Taille;
            VM_Etat =  vM_Etat;
            NavigationManager = navManager;
            _notificationPopUpService = notificationPopUpService;
            VM_Login = vM_Login;
        }

        private void NotifyStateChanged() => OnStateChange?.Invoke();

        public async Task InitializeFromUrlAsync()
        {
            var uri = NavigationManager.ToAbsoluteUri(NavigationManager.Uri);
            string? query = null;

            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("q", out var q))
                query = q;

            await InitializeAsync(query);
        }

        public async Task InitializeAsync(string? queryFromUrl)
        {
            Query = queryFromUrl;
            await LoadAsync();
        }
        
        private async Task LoadAsync()
        {
            IsLoading = true;
            NotifyStateChanged();

            await VM_Categorie.LoadAsync();
            await VM_Marque.LoadAsync();
            await VM_Genre.LoadAsync();
            await VM_Etat.LoadAsync();
            await VM_Taille.LoadAsync();
            
            await ApplyFilters();
            
            IsLoading = false;
            NotifyStateChanged();
        }

        #region Filter Actions
        public void ToggleCategory(int categoryId)
        {
            if (ExpandedCategories.Contains(categoryId))
                ExpandedCategories.Remove(categoryId);
            else
                ExpandedCategories.Add(categoryId);
            
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
            SelectedCategories.Clear();
            SelectedSousCategories.Clear();
            SelectedMarques.Clear();
            SelectedTailles.Clear();
            SelectedGenres.Clear();
            ExpandedCategories.Clear();
            CurrentPage = 1;
            SelectedPrice = SliderMax / 2;
            
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

        private async Task ApplyFilters()
        {
            foreach (var genre in  SelectedGenres )
            {
                Console.WriteLine("geeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeeere" + genre);
            }
            var filterRequest = new FilterDTO
            {
                MotCle = Query,
                Categories = SelectedCategories,
                SousCategories = SelectedSousCategories,
                Marques = SelectedMarques,
                Tailles = SelectedTailles,
                Genre = SelectedGenres,
                PrixMax = SelectedPrice,
                PrixMin = 0
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
            if (VM_Login.CheckLoginStatus == null)
            {
                NavigationManager.NavigateTo("/login");
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
                Categories = SelectedCategories,
                SousCategories = SelectedSousCategories,
                Marques = SelectedMarques,
                Tailles = SelectedTailles,
                Genre = SelectedGenres,
                PrixMax = SelectedPrice,
                PrixMin = 0
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
    }
}