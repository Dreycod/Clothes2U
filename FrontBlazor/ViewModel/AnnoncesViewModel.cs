using FrontBlazor.Models;
using FrontBlazor.Services;

namespace FrontBlazor.ViewModel
{
    public class AnnoncesViewModel
    {
        private readonly AnnonceService _service;

        public List<AnnonceDTO> Annonces { get; set; } = new List<AnnonceDTO>();

        public AnnoncesViewModel(AnnonceService service)
        {
            _service = service;
        }

        public async Task LoadAsync()
        {
            Annonces = await _service.GetActiveAnnoncesAsync();
        }

        public async Task<List<AnnonceDTO>> RecupererAnnoncesUtilisateur()
        {
            //Annonces = await _service.GetAnnoncesByIdUser
            Annonces.Add(new AnnonceDTO());
            return Annonces;
        }


        // Exclusive to search page, nouveautés et tendances
        public async Task<List<AnnonceDTO>> GetAnnoncesByFilters()
        {
            Annonces = await _service.GetActiveAnnoncesAsync();
            //Annonces = await _service.GetAnnoncesById
            return Annonces;
        }
    }
}