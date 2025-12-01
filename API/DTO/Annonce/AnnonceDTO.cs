namespace API.DTO.Annonce;

public class AnnonceDTO
{
    public int AnnonceId { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string NomMarque { get; set; } = null!;
    public int UtilisateurId { get; set; }
    public DateTime DateAnnonce { get; set; }
    public string EtatArticle { get; set; } = null!;
    public string Taille { get; set; } = null!;
    public List<string> Photos { get; set; } = new();
    public int NombreLikes { get; set; }
    public decimal Prix { get; set; }
    public string NomAuteur { get; set; } = null!;
    public string? UriPhotoProfilAuteur { get; set; } = null!;
    
}