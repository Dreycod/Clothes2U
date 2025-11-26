using FrontBlazor.Models;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.ViewModel
{
    public class AnnoncesViewModel
    {
        private readonly IAnnonceService<Annonce> _annonceService;

        public List<Annonce> Annonces { get; set; } = new List<Annonce>();

        public AnnoncesViewModel(IAnnonceService<Annonce> annonceService)
        {
            _annonceService = annonceService;
        }

        public async Task LoadAsync()
        {
            Annonces = new List<Annonce>();
            Annonces = await _annonceService.GetActiveAnnonces();
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
            Annonces = await _annonceService.GetActiveAnnonces();
            //Annonces = await _service.GetAnnoncesById
            return Annonces;
        }








        ////// Annonce Detail
        public Annonce annonceDetail { get; set; } = new Annonce();

        public async Task LoadAnnonceDetail(int id) // the page overrided to async Task this
        {
            try
            {
                Annonce annonceResult = await _annonceService.GetAnnonceDetailById(id);
                if (annonceResult != null)
                {
                    annonceDetail = annonceResult;
                }
            }
            catch (Exception ex)
            {
                //simulate annonce detail loading
                annonceDetail = new Annonce
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
}