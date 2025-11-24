namespace FrontBlazor.Models.Annonces
{
    public class AnnonceDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string NomMarque { get; set; }
        public string EtatArticle { get; set; }
        public string Taille { get; set; }
        public List<string> Photos { get; set; } = new();
        public int NombreLikes { get; set; }
        public decimal Prix { get; set; }
    }
}
