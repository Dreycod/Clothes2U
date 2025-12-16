using FrontBlazor.Models;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.ViewModel
{
    public class CreationAnnonceViewModel
    {
        private readonly IAnnonceService<Annonce> _annonceService;

        public List<Annonce> Annonces { get; set; } = new List<Annonce>();
        public bool IsLoading { get; set; }
        public string? ErrorMessage { get; set; }
        public CreationAnnonceViewModel(IAnnonceService<Annonce> annonceService, IFavorisService<Favoris> favorisService, IAuthService authService)
        {
            _annonceService = annonceService;
        }

        public async Task<Annonce?> CreateAnnonceAsync(Annonce newAnnonce)
        {
            IsLoading = true;
            ErrorMessage = null;
            try
            {
                var createdAnnonce = await _annonceService.AddAsync(newAnnonce);
                if (createdAnnonce != null)
                {
                    Annonces.Add(createdAnnonce);
                    return createdAnnonce;
                }
                return null;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erreur lors de la création de l'annonce";
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
