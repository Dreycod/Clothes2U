using System.Reflection;

namespace Shared.DTO.Annonce;

public class AnnonceDetailDTO
{
    public int AnnonceId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool Negociable { get; set; }
    public int UtilisateurId { get; set; }
    public string NomMarque { get; set; } = null!;
    public DateTime DateAnnonce { get; set; }
    public string EtatArticle { get; set; } = null!;
    public string Taille { get; set; } = null!;
    public List<int> Photos { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    public List<string> Couleurs { get; set; } = new();
    public int NombreLikes { get; set; } = 0;
    public int NombreVues { get; set; } = 0;
    public decimal Prix { get; set; }
    public string? SousCategorie { get; set; } 
    public string? Categorie { get; set; } 
    public bool IsLikedByCurrentUser { get; set; }
    public bool IsOwnerAnnonce { get; set; } = false;
    public string StatutAnnonce { get; set; }
    public int StatutAnnonceId { get; set; }

}