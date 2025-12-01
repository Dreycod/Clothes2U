using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Models;

namespace FrontBlazor.ViewModel
{
    public class MarqueViewModel
    {
        private readonly IMarqueService<Marque> _marqueService;
        public List<Marque> Marques { get; set; } = new();
        public bool IsLoading { get; set; } = false;
        public string? ErrorMessage { get; set; }

        public MarqueViewModel(IMarqueService<Marque> marqueService)
        {
            _marqueService = marqueService;
        }

        public async Task LoadMarquesAsync()
        {
            IsLoading = true;
            ErrorMessage = null;

            try
            {
                var result = await _marqueService.GetAllMarques();

                if (result != null)
                {
                    Marques = result;
                }
                else
                {
                    Marques = new List<Marque>();
                    ErrorMessage = "Impossible de charger les marques";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erreur lors du chargement des marques";
                Marques = new List<Marque>();
                Console.WriteLine($"LoadMarquesAsync Error: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
