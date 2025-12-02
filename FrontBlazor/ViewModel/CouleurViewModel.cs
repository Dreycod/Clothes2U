using FrontBlazor.Models;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.ViewModel
{
    public class CouleurViewModel
    {
        private readonly ICouleurService<Couleur> _couleurService;
        public List<Couleur> Couleurs { get; set; } = new();
        public bool IsLoading { get; set; } = false;
        public string? ErrorMessage { get; set; }

        public CouleurViewModel(ICouleurService<Couleur> couleurService)
        {
            _couleurService = couleurService;
        }

        public async Task LoadCouleursAsync()
        {
            IsLoading = true;
            ErrorMessage = null;

            try
            {
                var result = await _couleurService.GetAllCouleurs();

                if (result != null)
                {
                    Couleurs = result;
                }
                else
                {
                    Couleurs = new List<Couleur>();
                    ErrorMessage = "Impossible de charger les marques";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erreur lors du chargement des marques";
                Couleurs = new List<Couleur>();
                Console.WriteLine($"LoadMarquesAsync Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
