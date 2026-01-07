using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class MarqueManager : GenericCRUDManager<Marque>, IMarqueRepository
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

    public async Task<IEnumerable<Marque>> GetByString(string filter)
    {
        var query = BaseMarqueQuery();
        if (!string.IsNullOrEmpty(filter))
        {
            var lowerMotCle = filter.ToLower();
            query = query.Where(p =>
                p.NomMarque.ToLower().Contains(lowerMotCle));
        }
        return await query.ToListAsync();
    }
}