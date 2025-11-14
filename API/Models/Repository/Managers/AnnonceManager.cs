using API.Extensions;
using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class AnnonceManager : GenericCRUDManager<Annonce>, IAnnonceRepository<Annonce, int>
{
    public AnnonceManager(Clothes2UDbContext context) : base(context)
    {
    }
    private IQueryable<Annonce> BaseAnnonceQuery()
    {
        return _context.Annonces
            .Include(a => a.Marque)
            .Include(a => a.Statut)
            .Include(a => a.Taille)
            .Include(a => a.Etat)
            .Include(a => a.Photos)
            .ThenInclude(pa => pa.Photo)
            .Include(a => a.UtilisateursFavoris);
    }

    public override async Task<Annonce?> GetByIdAsync(int id)
    {
        return await BaseAnnonceQuery()
            .FirstOrDefaultAsync(a => a.AnnonceId == id);
    }


    public async Task<IEnumerable<Annonce>> GetByCategorieId(int id)
    {
        return await BaseAnnonceQuery()
            .Where(a => a.CategorieId == id)
            .ToListAsync();
    }

    public async Task<IEnumerable<Annonce>> GetBySousCategorieId(int id)
    {
        return await BaseAnnonceQuery()
            .Where(a => a.SousCategorieId == id)
            .ToListAsync();
    }
    public async Task<IEnumerable<Annonce>> GetActiveAnnonces()
    {
        return await BaseAnnonceQuery()
            .Where(a => a.Statut.StatutLibelle == "En Ligne") 
            .ToListAsync();
    }

}