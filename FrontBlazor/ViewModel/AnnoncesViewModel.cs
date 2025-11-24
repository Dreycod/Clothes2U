using FrontBlazor.Models.Annonces;
using FrontBlazor.Services;

namespace FrontBlazor.ViewModel
{
    public class AnnoncesViewModel
    {
        private readonly AnnonceService _service;

        public List<Annonce> Annonces { get; set; } = new List<Annonce>();

        public AnnoncesViewModel(AnnonceService service)
        {
            _service = service;
        }

        public async Task LoadAsync()
        {
            Annonces = await _service.GetActiveAnnoncesAsync();
        }

        public async Task<List<Annonce>> RecupererAnnoncesUtilisateur()
        {
            //Annonces = await _service.GetAnnoncesByIdUser
            Annonces.Add(new Annonce());
            return Annonces;
        }


        // Exclusive to search page, nouveautés et tendances
        public async Task<List<Annonce>> GetAnnoncesByFilters()
        {
            Annonces = await _service.GetActiveAnnoncesAsync();
            //Annonces = await _service.GetAnnoncesById
            return Annonces;
        }
    }
}