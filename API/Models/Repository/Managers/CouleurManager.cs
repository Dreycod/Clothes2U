using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class CouleurManager : GenericCRUDManager<Couleur>, ICaracteristiquesRepository<Couleur>
{
    public CouleurManager(Clothes2UDbContext context) : base(context) { }

    public async Task<IEnumerable<Couleur>> GetAllWithDetailsAsync()
    {
        return await BaseCouleurQuery().ToListAsync();
    }

    private IQueryable<Couleur> BaseCouleurQuery()
    {
        return _context.Couleurs
            .Include(a => a.Annonces)
            .AsSplitQuery();
    }
}