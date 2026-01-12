using Shared.DTO.Couleur;

namespace Shared.DTO.Annonce
{
    public class PutAnnonceDTO
    {
        public int AnnonceId { get; set; }
        public string Titre { get; set; }
        public string Description { get; set; }
        public DateTime DateAnnonce { get; set; }
        public bool EstNegociable { get; set; } = false;
        public decimal Prix { get; set; }
        public int UtilisateurId { get; set; }
        public int EtatId { get; set; }
        public int MarqueId { get; set; }
        public int TailleId { get; set; }
        public int SousCategorieId { get; set; }
        public int CategorieId { get; set; }
        public int StatutAnnonceId { get; set; } = 1;
        public List<int>? Couleurs { get; set; }
        public int GenreId { get; set; }
    }
}
