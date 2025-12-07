using FrontBlazor.Models;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.ViewModel
{
    public class AddViewModel
    {
        private readonly IAnnonceService<Annonce> _annonceService;
        private readonly IFavorisService<Favoris> _favorisService;
        private readonly IAuthService _authService;

        public List<Annonce> Annonces { get; set; } = new List<Annonce>();
        public Annonce? AnnonceDetail { get; set; }
        public bool IsLoading { get; set; }
        public string? ErrorMessage { get; set; }

        public AddViewModel(IAnnonceService<Annonce> annonceService, IFavorisService<Favoris> favorisService, IAuthService authService)
        {
            _annonceService = annonceService;
            _favorisService = favorisService;
            _authService = authService;
        }

        public async Task<bool> CreateAnnonceAsync(Annonce newAnnonce)
        {
            IsLoading = true;
            ErrorMessage = null;
            try
            {
                var createdAnnonce = await _annonceService.AddAsync(newAnnonce);
                if (createdAnnonce != null)
                {
                    Annonces.Add(createdAnnonce);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erreur lors de la création de l'annonce";
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
            finally
            {
                IsLoading = false;
            }
        }
   }
}
