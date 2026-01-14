using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public interface IFavorisRepository : IDataRepository<Favoris, int>
{
    Task<Favoris> GetFavorisByAnnonceAndUserId(int UtilisateurId, int AnnonceID);
    Task<bool> CheckIfLiked(int utilisateurId, int annonceId);
    Task<IEnumerable<Utilisateur>> GetUtilisateurByAnnonceId(int annonceId);
}