namespace FrontBlazor.Models
{
    public class Annonce: IEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string NomMarque { get; set; }
        public string EtatArticle { get; set; }
        public string Taille { get; set; }
        public List<string> Photos { get; set; } = new();
        public int NombreLikes { get; set; }
        public decimal Prix { get; set; }
        public int GetId()
        {
            return Id;
        }
    }
}
