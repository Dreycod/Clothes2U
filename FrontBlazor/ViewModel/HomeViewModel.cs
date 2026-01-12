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
    public class HomeViewModel :  ClientBaseViewModel
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
        public List<AnnonceDTO> AnnoncesRecommended { get; set; }
        #endregion


        #region recents
        public bool IsLoadingNextRecents { get; set; } = false;
        public bool IsLoadingPreviousRecents { get; set; } = false;
        public int PageRecents { get; set; } = 1;
        public int PageSizeRecents { get; set; } = 10;
        public List<AnnonceDTO> AnnoncesRecents { get; set; }
        #endregion

        #region populaires
        public bool IsLoadingNextPopulaires { get; set; } = false;
        public bool IsLoadingPreviousPopulaires { get; set; } = false;
        public int PagePopulaires { get; set; } = 1;
        public int PageSizePopulaires { get; set; } = 10;
        public List<AnnonceDTO> AnnoncesPopulaires { get; set; }
        #endregion
        public string? ErrorMessage { get; set; }
        public string SuccessMessage { get; set; } = string.Empty;

        public HomeViewModel(
            IAnnonceService annonceService,
            IFavorisService<FavorisDTO> favorisService,
            IAuthService authService,
            NavigationManager navigationManager,
            INotificationService notificationService
            )
        : base(navigationManager, authService, notificationService)
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
            await  base.LoadAsync();
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

        public async Task PreviousRecommandation()
        {
            if (PageRecommandation > 1)
            {
                IsLoadingPreviousRecommendation = true;
                NotifyStateChanged();
                PageRecommandation--;
                AnnoncesRecommended = await _annonceService.GetRecommendedAnnonces(PageRecommandation, PageSizeRecommandation);
                IsLoadingPreviousRecommendation = false;
                NotifyStateChanged();
            }
        }

        public async Task NextRecommandation()
        {
            if (AnnoncesRecommended.Count == PageSizeRecommandation)
            {
                IsLoadingNextRecommendation = true;
                List<AnnonceDTO> NewAnnoncesRecommended = await _annonceService.GetRecommendedAnnonces(PageRecommandation + 1, PageSizeRecommandation);
                if (NewAnnoncesRecommended.Count > 0)
                {
                    PageRecommandation++;
                    AnnoncesRecommended = NewAnnoncesRecommended;
                    IsLoadingNextRecommendation = false;
                    NotifyStateChanged();
                }
            }
        }

        public async Task NextRecents()
        {
            if (AnnoncesRecents.Count == PageSizeRecents)
            {
                IsLoadingNextRecents = true;
                NotifyStateChanged();
                List<AnnonceDTO> NewAnnoncesRecents = await _annonceService.GetAnnonceByFilter(new FilterDTO
                {
                    SortBy = SortField.DateAnnonce,
                    SortOrder = SortOrder.Descending,
                }, PageRecents + 1, PageSizeRecents);
                if (NewAnnoncesRecents.Count > 0)
                {
                    PageRecents++;
                    AnnoncesRecents = NewAnnoncesRecents;
                    IsLoadingNextRecents = false;
                    NotifyStateChanged();
                }
            }
        }
        public async Task PreviousRecents()
        {
            if (PageRecents > 1)
            {
                IsLoadingPreviousRecents = true;
                NotifyStateChanged();
                PageRecents--;
                AnnoncesRecents = await _annonceService.GetAnnonceByFilter(new FilterDTO
                {
                    SortBy = SortField.DateAnnonce,
                    SortOrder = SortOrder.Descending,
                }, PageRecents, PageSizeRecents);
                IsLoadingPreviousRecents = false;
                NotifyStateChanged();
            }
        }

        public async Task NextPopulaires()
        {
            if (AnnoncesPopulaires.Count == PageSizePopulaires)
            {
                IsLoadingNextPopulaires = true;
                NotifyStateChanged();
                List<AnnonceDTO> NewAnnoncesPopulaires = await _annonceService.GetAnnonceByFilter(new FilterDTO
                {
                    SortBy = SortField.NombreFavoris,
                    SortOrder = SortOrder.Descending,
                }, PagePopulaires + 1, PageSizePopulaires);
                if (NewAnnoncesPopulaires.Count > 0)
                {
                    PagePopulaires++;
                    AnnoncesPopulaires = NewAnnoncesPopulaires;
                    IsLoadingNextPopulaires = false;
                    NotifyStateChanged();
                }
            }

        }
        public async Task PreviousPopulaires()
        {
            if (PagePopulaires > 1)
            {
                IsLoadingPreviousPopulaires = true;
                NotifyStateChanged();
                PagePopulaires--;
                AnnoncesPopulaires = await _annonceService.GetAnnonceByFilter(new FilterDTO
                {
                    SortBy = SortField.NombreFavoris,
                    SortOrder = SortOrder.Descending,
                }, PagePopulaires, PageSizePopulaires);
                IsLoadingPreviousPopulaires = false;
                NotifyStateChanged();
            }
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

            var annonce = AnnoncesRecents.FirstOrDefault(a => a.AnnonceId == annonceId) ?? AnnoncesPopulaires.FirstOrDefault(a => a.AnnonceId == annonceId);
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