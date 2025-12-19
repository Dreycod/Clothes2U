using Shared.DTO;
using Shared.DTO.Annonce;
using Shared.DTO.Favoris;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FrontBlazor.ViewModel
{
    public class HomeViewModel
    {
        private readonly IAnnonceService<AnnonceDTO> _annonceService;
        private readonly IFavorisService<FavorisDTO> _favorisService;
        private readonly IAuthService _authService;
        private readonly NavigationManager _navigationManager;

        public List<AnnonceDTO> Annonces { get; set; } = new List<AnnonceDTO>();
        public AnnonceDTO? AnnonceDetail { get; set; }
        public bool IsLoading { get; set; }
        public string? ErrorMessage { get; set; }
        public string SuccessMessage { get; set; } = string.Empty;

        public HomeViewModel(
            IAnnonceService<AnnonceDTO> annonceService,
            IFavorisService<FavorisDTO> favorisService,
            IAuthService authService,
            NavigationManager navigationManager)
        {
            _annonceService = annonceService;
            _favorisService = favorisService;
            _authService = authService;
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

        public async Task LoadRecentAnnonces()
        {
            ErrorMessage = SuccessMessage = string.Empty;
            IsLoading = true;

            FilterDTO filter = new FilterDTO
            {
                SortBy = SortField.DateAnnonce,
                SortOrder = SortOrder.Descending,
            };

            try
            {
                List<AnnonceDTO> result = await GetAnnoncesByFiltreAsync(filter, 1, 10);

                if (result != null && result.Any())
                {
                    Annonces = result;
                    SuccessMessage = "Chargement réussi.";

                    // Print all of result in console for debugging
                    foreach (AnnonceDTO annonce in result)
                    {
                        Console.WriteLine($"Annonce ID: {annonce.AnnonceId}, Title: {annonce.Titre}");
                    }
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

        public void NavigateToProductDetail(int? productId)
        {
            if (productId.HasValue)
            {
                _navigationManager.NavigateTo($"/product/{productId}");
            }
        }

        public async Task ToggleFavoriteWithAuth(int annonceId, bool isUserLoggedIn, Action navigateToLogin)
        {
            if (!isUserLoggedIn)
            {
                navigateToLogin();
                return;
            }

            var annonce = Annonces.FirstOrDefault(a => a.AnnonceId == annonceId);
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
    }
}