using Shared.Interfaces;

namespace Shared.DTO.Favoris;

public class FavorisDTO : IEntity
{
    public int AnnonceId { get; set; }
    public int UtilisateurId { get; set; }

    public int GetId()
    {
        return AnnonceId;
    }
}