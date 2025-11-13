namespace API.DTO.Annonce;

public class AnnonceDTO
{
    public int Id { get; set; }
    public string NomMarque { get; set; } = null!;
    public string EtatArticle { get; set; } = null!;
    public string Taille { get; set; } = null!;
    public List<string> Photos { get; set; } = new();
    public int NombreLikes { get; set; }
    public decimal Prix { get; set; }
    
}