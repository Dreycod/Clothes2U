using Shared.Interfaces;

namespace Shared.DTO.Annonce;


public class AnnonceDTO : CreateAnnonceDTO, IEntity
{
    public int AnnonceId { get; set; }
    public string NomMarque { get; set; } = null!;
    public bool IsLikedByCurrentUser { get; set; }
    public string EtatArticle { get; set; } = null!;
    public string Taille { get; set; } = null!;
    public List<int> Photos { get; set; } = new();
    public int NombreLikes { get; set; }
    public int NombreVues { get; set; }
    public string NomAuteur { get; set; } = null!;
    public string? UriPhotoProfilAuteur { get; set; } = null!;
}