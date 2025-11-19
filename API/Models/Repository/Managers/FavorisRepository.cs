using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class FavorisRepository : GenericCRUDManager<Favoris>,  IFavorisRepository
{
    public FavorisRepository(Clothes2UDbContext context) :  base(context){}

    public async  Task<Favoris> GetFavorisByAnnonceAndUserId(int UtilisateurId, int AnnonceId)
    {
        return await _context.Favorises
            .FirstOrDefaultAsync(f => f.UtilisateurId == UtilisateurId && f.AnnonceId == AnnonceId);
    }
}