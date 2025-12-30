using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class TailleManager : GenericCRUDManager<Taille>, ITailleRepository
{
    public TailleManager(Clothes2UDbContext context) : base(context) { }

    public async Task<IEnumerable<Taille>> GetAllAsyncByIdentifier(int id)
    {
        return await _context.Tailles.Where(e => e.TailleId == id).ToListAsync();
    }

    public async Task<IEnumerable<Taille>> GetAllWithDetailsAsync()
    {
        return await BaseTailleQuery().ToListAsync();
    }

    private IQueryable<Taille> BaseTailleQuery()
    {
        return _context.Tailles
            .Include(a => a.Annonces)
            .AsSplitQuery();
    }
}