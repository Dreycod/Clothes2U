using FrontBlazor.Attributes;
using System.Text.Json.Serialization;

namespace FrontBlazor.Models;

public class Annonce: IEntity
{
    [IgnoreInTemplate]
    public int AnnonceId { get; set; } 
    public string Title { get; set; } = null!;
    public bool Negociable { get; set; }
    public int UtilisateurId { get; set; }
    public decimal Prix { get; set; }
    public string? Description { get; set; } = null!;
    [IgnoreInTemplate]
    public string? NomMarque { get; set; } = null!;
    [IgnoreInTemplate]
    public DateTime DateAnnonce { get; set; }
    [IgnoreInTemplate]
    public bool IsLikedByCurrentUser { get; set; }
    [IgnoreInTemplate]
    public string? EtatArticle { get; set; } = null!;
    [IgnoreInTemplate]
    public string? Taille { get; set; } = null!;
    [IgnoreInTemplate]
    public List<int>? Photos { get; set; } = new();
    [IgnoreInTemplate]
    public int NombreLikes { get; set; } = 0;
    [IgnoreInTemplate]
    public string NomAuteur { get; set; } = null!;
    [IgnoreInTemplate]
    public string? UriPhotoProfilAuteur { get; set; } = null!;
    [IgnoreInTemplate]
    public string? SousCategorie { get; set; }
    [IgnoreInTemplate]
    public string? Categorie { get; set; }

    //id pour les posts : 

    public int? EtatId { get; set; }
    public int? MarqueId { get; set; }
    public int? TailleId { get; set; }
    public int? SousCategorieId { get; set; }
    public int? CategorieId { get; set; }
    public int? StatutAnnonceId { get; set; }
    public int? GenreId { get; set; }
    public int GetId()
    {
        return AnnonceId;
    }
}
