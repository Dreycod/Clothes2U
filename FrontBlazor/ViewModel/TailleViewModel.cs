using FrontBlazor.Models;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.ViewModel
{
    public class TailleViewModel
    {
        private readonly ITailleService<Taille> _tailleService;
        public List<Taille> Tailles { get; set; } = new();
        public bool IsLoading { get; set; } = false;
        public string? ErrorMessage { get; set; }

        public TailleViewModel(ITailleService<Taille> tailleService)
        {
            _tailleService = tailleService;
        }

        public async Task LoadTaillesAsync()
        {
            IsLoading = true;
            ErrorMessage = null;

            try
            {
                var result = await _tailleService.GetAllTailles();

                if (result != null)
                {
                    Tailles = result;
                }
                else
                {
                    Tailles = new List<Taille>();
                    ErrorMessage = "Impossible de charger les tailles";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erreur lors du chargement des tailles";
                Tailles = new List<Taille>();
                Console.WriteLine($"LoadTailelsAsync Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
