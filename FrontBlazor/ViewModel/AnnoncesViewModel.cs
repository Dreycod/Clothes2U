using FrontBlazor.Models;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.ViewModel
{
    // ViewModel - UI logic only
    public class AnnoncesViewModel
    {
        private readonly IAnnonceService<Annonce> _annonceService;

        // Observable properties
        public List<Annonce> Annonces { get; set; } = new();
        public Annonce? AnnonceDetail { get; set; }
        public bool IsLoading { get; set; }
        public string? ErrorMessage { get; set; }

        public AnnoncesViewModel(IAnnonceService<Annonce> annonceService)
        {
            _annonceService = annonceService;
        }

        // UI-focused methods
        public async Task LoadActiveAnnoncesAsync()
        {
            IsLoading = true;
            ErrorMessage = null;

            try
            {
                Annonces = await _annonceService.GetActiveAnnonces() ?? new();
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
    }
}