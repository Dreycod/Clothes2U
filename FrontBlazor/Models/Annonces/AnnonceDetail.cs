namespace FrontBlazor.Models.Annonces
{
    public class AnnonceDetail
    {
        public int? AnnonceId { get; set; }
        public string Title { get; set; }
        public bool Negociable { get; set; }
        public int UtilisateurId { get; set; }
        public string? NomMarque { get; set; } = null!;
        public DateTime DateAnnonce { get; set; }
        public string? EtatArticle { get; set; } = null!;
        public string? Taille { get; set; } = null!;
        public List<string>? Photos { get; set; } = new();
        public int NombreLikes { get; set; } = 0;
        public decimal Prix { get; set; }



        public string? SousCategorie { get; set; }
        public string? Categorie { get; set; }

        //id pour les posts : 

        public int EtatId { get; set; }
        public int MarqueId { get; set; }
        public int TailleId { get; set; }
        public int SousCategorieId { get; set; }
        public int CategorieId { get; set; }
        public int StatutAnnonceId { get; set; }
    }
}
