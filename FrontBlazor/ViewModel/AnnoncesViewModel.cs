using FrontBlazor.Models;
using FrontBlazor.Services;

namespace FrontBlazor.ViewModel
{
    public class AnnoncesViewModel
    {
        private readonly AnnonceService _service;

        public List<AnnonceDTO> Annonces { get; private set; } = new();

        public AnnoncesViewModel(AnnonceService service)
        {
            _service = service;
        }

        public async Task LoadAsync()
        {
            Annonces = await _service.GetActiveAnnoncesAsync();
        }
    }

}
