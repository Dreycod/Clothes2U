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
            Annonces = new List<Annonce>();
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