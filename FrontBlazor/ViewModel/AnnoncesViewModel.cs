using FrontBlazor.Models;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.ViewModel
{
    public class AnnoncesViewModel
    {
        private readonly IAnnonceService<Annonce> _annonceService;
        private readonly IFavorisService<Favoris> _favorisService;
        private readonly IAuthService _authService;

        public List<Annonce> Annonces { get; set; } = new List<Annonce>();
        public Annonce? AnnonceDetail { get; set; }
        public bool IsLoading { get; set; }
        public string? ErrorMessage { get; set; }

        public AnnoncesViewModel(IAnnonceService<Annonce> annonceService, IFavorisService<Favoris> favorisService, IAuthService authService)
        {
            _annonceService = annonceService;
            _favorisService = favorisService;
            _authService = authService;
        }

        public async Task LoadActiveAnnoncesAsync()
        {
            IsLoading = true;
            ErrorMessage = null;

            try
            {
                Annonces = await _annonceService.GetActiveAnnonces() ?? new List<Annonce>();
                foreach (var annonce in Annonces)
                {
                    Console.WriteLine(annonce.IsLikedByCurrentUser);    
                }
                
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erreur lors du chargement des annonces";
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task LoadAnnonceDetailAsync(int id)
        {
            IsLoading = true;
            ErrorMessage = null;

            try
            {
                AnnonceDetail = await _annonceService.GetAnnonceDetailById(id);
                if (AnnonceDetail == null)
                {
                    ErrorMessage = "Annonce introuvable";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erreur lors du chargement de l'annonce";
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
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

        public async Task ToggleFavorite(bool isLiked, int annonceId)
        {
            if (!isLiked)
                await _favorisService.AddFavoris(annonceId);
            else
                await _favorisService.DeleteFavoris(annonceId);


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
    }
}