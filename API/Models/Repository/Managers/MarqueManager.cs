using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class MarqueManager : GenericCRUDManager<Marque>, ICaracteristiquesRepository<Marque>
{
    public MarqueManager(Clothes2UDbContext context) : base(context){}

    public async Task<IEnumerable<Marque>> GetAllWithDetailsAsync()
    {
        return await BaseMarqueQuery().ToListAsync();
    }

    private IQueryable<Marque> BaseMarqueQuery()
    {
        return _context.Marques
            .Include(a => a.Annonces)
            .AsSplitQuery();
    }
}