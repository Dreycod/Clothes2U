using FrontBlazor.Models;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;

namespace FrontBlazor.ViewModel
{
    public class HomeViewModel
    {
        private readonly IAnnonceService<Annonce> _annonceService;
        private readonly IFavorisService<Favoris> _favorisService;
        private readonly IAuthService _authService;

        public List<Annonce> Annonces { get; set; } = new List<Annonce>();
        public Annonce? AnnonceDetail { get; set; }
        public bool IsLoading { get; set; }
        public string? ErrorMessage { get; set; }

        public HomeViewModel(IAnnonceService<Annonce> annonceService, IFavorisService<Favoris> favorisService, IAuthService authService)
        {
            _annonceService = annonceService;
            _favorisService = favorisService;
            _authService = authService;
        }
        public async Task<List<Annonce>?> GetAnnoncesByFiltreAsync(FilterDTO filterDto, int page = 1, int pageSize = 3)
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

        public async Task ToggleFavorite(bool isLiked, int annonceId)
        {
            if (!isLiked)
                await _favorisService.AddFavoris(annonceId);
            else
                await _favorisService.DeleteFavoris(annonceId);


        }


    }
}