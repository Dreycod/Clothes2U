using FrontBlazor.Models;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.ViewModel
{
    public class AnnoncesViewModel
    {
        private readonly IAnnonceService<Annonce> _annonceService;

        public List<Annonce> Annonces { get; set; } = new List<Annonce>();
        public Annonce? AnnonceDetail { get; set; }
        public bool IsLoading { get; set; }
        public string? ErrorMessage { get; set; }

        public AnnoncesViewModel(IAnnonceService<Annonce> annonceService)
        {
            _annonceService = annonceService;
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
        public async Task CreateAnnonceAsync(Annonce newAnnonce)
        {
            IsLoading = true;
            ErrorMessage = null;
            try
            {
                var createdAnnonce = await _annonceService.AddAsync(newAnnonce);
                if (createdAnnonce != null)
                {
                    Annonces.Add(createdAnnonce);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erreur lors de la création de l'annonce";
                Console.WriteLine($"Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}