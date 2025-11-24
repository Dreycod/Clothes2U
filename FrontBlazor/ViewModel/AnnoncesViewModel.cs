using FrontBlazor.Models.Annonces;
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
            Annonces = new List<AnnonceDTO>();
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








        ////// Annonce Detail
        public AnnonceDetailDTO annonceDetailDTO { get; set; } = new AnnonceDetailDTO();
        private AnnonceService _annonceService;

        public void LoadAnnonceDetail(int id) // the page overrided to async Task this
        {
            //AnnonceDetailDTO annonce = await _annonceService.GetAnnonceDetailById(id).Result;
            //simulate annonce detail loading
            annonceDetailDTO = new AnnonceDetailDTO
            {
                AnnonceId = id,
                Title = "Veste en cuir vintage",
                Negociable = true,
                UtilisateurId = 3,
                NomMarque = "VintageCo",
                DateAnnonce = DateTime.Now.AddDays(-5),
                EtatArticle = "Bon état",
                Taille = "M",
                Photos = new List<string>
                {
                    "https://example.com/photos/veste1.jpg",
                    "https://example.com/photos/veste2.jpg"
                },
                NombreLikes = 27,
                Prix = 120.00m,
                SousCategorie = "Vestes",
                Categorie = "Vêtements",
            };
        }
    }
}