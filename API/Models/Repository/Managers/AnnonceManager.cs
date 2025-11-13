using API.Extensions;
using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class AnnonceManager : GenericCRUDManager<Annonce>, IAnnonceRepository<Annonce, int>
{
    public AnnonceManager(Clothes2UDbContext context) : base(context)
    {
    }
    public async Task<IEnumerable<Annonce>> GetByCategorieId(int id)
    {
        return await _context.Annonces
            .Where(a => a.CategorieId == id)
            .Include(a => a.Marque)
            .Include(a => a.Etat)
            .Include(a => a.Taille)
            .Include(a => a.Photos)
            .ThenInclude(pa => pa.Photo)
            .Include(a => a.UtilisateursFavoris)
            .ToListAsync();
    }
    public async Task<IEnumerable<Annonce>> GetBySousCategorieId(int id)
    {
        return await _context.Annonces.Where(a => a.SousCategorieId == id).ToListAsync();
    }
}