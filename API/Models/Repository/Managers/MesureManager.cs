using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers
{
    public class MesureManager : GenericCRUDManager<Mesure>, ICaracteristiquesRepository<Mesure>
    {
        public MesureManager(Clothes2UDbContext context) : base(context) { }

        public async Task<IEnumerable<Mesure>> GetAllWithDetailsAsync()
        {
            return await BaseMesureQuery().ToListAsync();
        }

        private IQueryable<Mesure> BaseMesureQuery()
        {
            return _context.Mesures
                .Include(a => a.SousCategorieMesure)
                .Include(a => a.TailleMesure)
                .AsSplitQuery();
        }
    }
}
