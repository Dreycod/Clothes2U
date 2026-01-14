using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class FavorisManager : GenericCRUDManager<Favoris>,  IFavorisRepository
{
    public FavorisManager(Clothes2UDbContext context) :  base(context){}

    private IQueryable<Favoris> BaseFavorisQuery()
    {
        return _context.Favorises
            .Include(f => f.Utilisateur)
            .AsSplitQuery();
    }

    public async  Task<Favoris> GetFavorisByAnnonceAndUserId(int UtilisateurId, int AnnonceId)
    {
        return await _context.Favorises
            .FirstOrDefaultAsync(f => f.UtilisateurId == UtilisateurId && f.AnnonceId == AnnonceId);
    }

    public async Task<bool> CheckIfLiked(int utilisateurId, int annonceId)
    {
        Favoris? postIsLiked = await _context.Favorises.FirstOrDefaultAsync(f => f.UtilisateurId == utilisateurId && f.AnnonceId == annonceId);
        return postIsLiked != null;
    }

    public async Task<IEnumerable<Utilisateur>> GetUtilisateurByAnnonceId(int annonceId)
    {
        return await BaseFavorisQuery()
            .Where(f => f.AnnonceId == annonceId).Select(f => f.Utilisateur).ToListAsync();
    }
}