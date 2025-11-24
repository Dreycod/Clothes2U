using FrontBlazor.Models;
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
            Annonces = await _service.GetActiveAnnonces();
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
            Annonces = await _service.GetActiveAnnonces();
            //Annonces = await _service.GetAnnoncesById
            return Annonces;
        }








        ////// Annonce Detail
        public AnnonceDetail annonceDetail { get; set; } = new AnnonceDetail();
        private AnnonceService _annonceService;

        public async void LoadAnnonceDetail(int id) // the page overrided to async Task this
        {
            AnnonceDetail annonceResult = await _annonceService.GetAnnonceDetailById(id);
            if (annonceResult != null)
            {
                annonceDetail = annonceResult;
                return;
            }
            //simulate annonce detail loading
            annonceDetail = new AnnonceDetail
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