using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class TailleManager : GenericCRUDManager<Taille>, IDataRepository<Taille, int>
{
    public TailleManager(Clothes2UDbContext context) : base(context) { }
    private IQueryable<Taille> BaseTailleQuery()
    {
        return _context.Tailles
            .Include(a => a.Annonces)
            .Include(a => a.Mesures)
            .AsSplitQuery();
    }
}