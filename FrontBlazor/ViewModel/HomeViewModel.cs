using FrontBlazor.Services;
using FrontBlazor.Services.Interfaces;
using FrontBlazor.ViewModel.Generic;
using Microsoft.AspNetCore.Components;
using Shared.DTO;
using Shared.DTO.Annonce;
using Shared.DTO.Favoris;
using Shared.DTO.Utilisateur;

namespace FrontBlazor.ViewModel
{
    public class HomeViewModel : ClientBaseViewModel
    {
        private readonly IAnnonceService _annonceService;
        private readonly IFavorisService<FavorisDTO> _favorisService;
        private readonly NavigationManager _navigationManager;
        private SearchAnnonceViewModel? _searchViewModel;

        #region recommandation 
        public bool IsLoadingNextRecommendation { get; set; } = false;
        public bool IsLoadingPreviousRecommendation { get; set; } = false;
        public int PageRecommandation { get; set; } = 1;
        public int PageSizeRecommandation { get; set; } = 15;
        public List<AnnonceDTO>? AnnoncesRecommended { get; set; }
        #endregion

        #region recents
        public bool IsLoadingNextRecents { get; set; } = false;
        public bool IsLoadingPreviousRecents { get; set; } = false;
        public int PageRecents { get; set; } = 1;
        public int PageSizeRecents { get; set; } = 10;
        public List<AnnonceDTO>? AnnoncesRecents { get; set; }
        #endregion

        #region populaires
        public bool IsLoadingNextPopulaires { get; set; } = false;
        public bool IsLoadingPreviousPopulaires { get; set; } = false;
        public int PagePopulaires { get; set; } = 1;
        public int PageSizePopulaires { get; set; } = 10;
        public List<AnnonceDTO>? AnnoncesPopulaires { get; set; }
        #endregion

        public string? ErrorMessage { get; set; }
        public string SuccessMessage { get; set; } = string.Empty;

        public HomeViewModel(
            IAnnonceService annonceService,
            IFavorisService<FavorisDTO> favorisService,
            IAuthService authService,
            NavigationManager navigationManager,
            ISignalRService notificationHubService,
            INotificationService notificationService
            )
        : base(navigationManager, authService, notificationHubService, notificationService)
        {
            _annonceService = annonceService;
            _favorisService = favorisService;
            _navigationManager = navigationManager;
        }

        public async Task<List<AnnonceDTO>?> GetAnnoncesByFiltreAsync(FilterDTO filterDto, int page = 1, int pageSize = 3)
        {
            try
            {
                return await _annonceService.GetAnnonceByFilter(filterDto, page, pageSize);
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erreur lors du filtrage des annonces";
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }

        public async Task SetSearchViewModel(SearchAnnonceViewModel searchViewModel)
        {
            _searchViewModel = searchViewModel;
        }

        public override async Task LoadAsync()
        {
            await base.LoadAsync();
            ErrorMessage = SuccessMessage = string.Empty;
            PageRecommandation = 1;
            await LoadRecentAnnonces();
            await LoadPopularAnnonces();
            if (IsLoggedIn)
            {
                AnnoncesRecommended = await _annonceService.GetRecommendedAnnonces(PageRecommandation, PageSizeRecommandation);
            }
            IsLoading = false;
        }

        private async Task NavigateCarousel(
            int currentPage,
            int pageSize,
            List<AnnonceDTO>? currentList,
            Func<int, int, Task<List<AnnonceDTO>>> loadDataFunc,
            Action<bool> setLoadingState,
            Action<int> updatePage,
            Action<List<AnnonceDTO>?> updateList,
            bool isNext)
        {
            if (isNext)
            {
                // Pour aller plus loin, vérifier qu'on a une page complète
                if (currentList == null || currentList.Count < pageSize)
                    return;
            }
            else
            {
                // Pour revenir en arrière, vérifier qu'on n'est pas déjà à la page 1
                if (currentPage <= 1)
                    return;
            }

            // Activer l'état de chargement
            setLoadingState(true);

            // Effacer la liste actuelle pour montrer le spinner
            updateList(null);
            NotifyStateChanged();

            try
            {
                // Calculer la nouvelle page
                int newPage = isNext ? currentPage + 1 : currentPage - 1;

                // Charger les nouvelles données
                List<AnnonceDTO> newAnnonces = await loadDataFunc(newPage, pageSize);

                // Si on a des résultats (ou si on revient en arrière), mettre à jour
                if (newAnnonces.Count > 0 || !isNext)
                {
                    updatePage(newPage);
                    updateList(newAnnonces);
                }
                else
                {
                    // Si pas de résultats, restaurer la liste précédente
                    updateList(currentList);
                }
            }
            finally
            {
                // Désactiver l'état de chargement
                setLoadingState(false);
                NotifyStateChanged();
            }
        }

        public async Task PreviousRecommandation()
        {
            await NavigateCarousel(
                PageRecommandation,
                PageSizeRecommandation,
                AnnoncesRecommended,
                async (page, size) => await _annonceService.GetRecommendedAnnonces(page, size),
                isLoading => IsLoadingPreviousRecommendation = isLoading,
                page => PageRecommandation = page,
                list => AnnoncesRecommended = list,
                isNext: false
            );
        }

        public async Task NextRecommandation()
        {
            await NavigateCarousel(
                PageRecommandation,
                PageSizeRecommandation,
                AnnoncesRecommended,
                async (page, size) => await _annonceService.GetRecommendedAnnonces(page, size),
                isLoading => IsLoadingNextRecommendation = isLoading,
                page => PageRecommandation = page,
                list => AnnoncesRecommended = list,
                isNext: true
            );
        }

        public async Task NextRecents()
        {
            await NavigateCarousel(
                PageRecents,
                PageSizeRecents,
                AnnoncesRecents,
                async (page, size) => await _annonceService.GetAnnonceByFilter(new FilterDTO
                {
                    SortBy = SortField.DateAnnonce,
                    SortOrder = SortOrder.Descending,
                }, page, size),
                isLoading => IsLoadingNextRecents = isLoading,
                page => PageRecents = page,
                list => AnnoncesRecents = list,
                isNext: true
            );
        }

        public async Task PreviousRecents()
        {
            await NavigateCarousel(
                PageRecents,
                PageSizeRecents,
                AnnoncesRecents,
                async (page, size) => await _annonceService.GetAnnonceByFilter(new FilterDTO
                {
                    SortBy = SortField.DateAnnonce,
                    SortOrder = SortOrder.Descending,
                }, page, size),
                isLoading => IsLoadingPreviousRecents = isLoading,
                page => PageRecents = page,
                list => AnnoncesRecents = list,
                isNext: false
            );
        }

        public async Task NextPopulaires()
        {
            await NavigateCarousel(
                PagePopulaires,
                PageSizePopulaires,
                AnnoncesPopulaires,
                async (page, size) => await _annonceService.GetAnnonceByFilter(new FilterDTO
                {
                    SortBy = SortField.NombreFavoris,
                    SortOrder = SortOrder.Descending,
                }, page, size),
                isLoading => IsLoadingNextPopulaires = isLoading,
                page => PagePopulaires = page,
                list => AnnoncesPopulaires = list,
                isNext: true
            );
        }

        public async Task PreviousPopulaires()
        {
            await NavigateCarousel(
                PagePopulaires,
                PageSizePopulaires,
                AnnoncesPopulaires,
                async (page, size) => await _annonceService.GetAnnonceByFilter(new FilterDTO
                {
                    SortBy = SortField.NombreFavoris,
                    SortOrder = SortOrder.Descending,
                }, page, size),
                isLoading => IsLoadingPreviousPopulaires = isLoading,
                page => PagePopulaires = page,
                list => AnnoncesPopulaires = list,
                isNext: false
            );
        }

        public async Task LoadRecentAnnonces()
        {
            FilterDTO filter = new FilterDTO
            {
                SortBy = SortField.DateAnnonce,
                SortOrder = SortOrder.Descending,
            };
            try
            {
                List<AnnonceDTO> result = await GetAnnoncesByFiltreAsync(filter, PageRecents, PageSizeRecents);

                if (result != null && result.Any())
                {
                    AnnoncesRecents = result;
                    SuccessMessage = "Chargement reussi.";
                }
                else
                {
                    ErrorMessage = "Erreur lors du chargement d'annonces recent.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task LoadPopularAnnonces()
        {
            FilterDTO filter = new FilterDTO
            {
                SortBy = SortField.NombreFavoris,
                SortOrder = SortOrder.Descending,
            };

            try
            {
                List<AnnonceDTO> result = await GetAnnoncesByFiltreAsync(filter, PagePopulaires, PageSizePopulaires);

                if (result != null && result.Any())
                {
                    AnnoncesPopulaires = result;
                    SuccessMessage = "Chargement reussi.";
                }
                else
                {
                    ErrorMessage = "Erreur lors du chargement d'annonces populaire.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Erreur: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void NavigateToProductDetail(int? productId)
        {
            if (productId.HasValue)
            {
                _navigationManager.NavigateTo($"/product/{productId}");
            }
        }

        public async Task ToggleFavoriteWithAuth(int annonceId, Action navigateToLogin)
        {
            if (utilisateur == null)
            {
                _navigationManager.NavigateTo("/login");
                return;
            }

            var annonce = AnnoncesRecents?.FirstOrDefault(a => a.AnnonceId == annonceId)
                ?? AnnoncesPopulaires?.FirstOrDefault(a => a.AnnonceId == annonceId);
            if (annonce == null) return;

            await ToggleFavorite(annonce.IsLikedByCurrentUser, annonceId);

            annonce.IsLikedByCurrentUser = !annonce.IsLikedByCurrentUser;
        }

        public async Task ToggleFavorite(bool isLiked, int annonceId)
        {
            if (!isLiked)
                await _favorisService.AddFavoris(annonceId);
            else
                await _favorisService.DeleteFavoris(annonceId);
        }

        public async Task OnGenreSelected(string genre)
        {
            _navigationManager.NavigateTo($"/search?genre={Uri.EscapeDataString(genre)}");
        }

        public async Task<bool> CheckIfOwnerAnnonce(AnnonceDTO annonce)
        {
            if (utilisateur == null)
                return false;

            return annonce.IdAuteur == utilisateur.UtilisateurId;
        }
    }
}