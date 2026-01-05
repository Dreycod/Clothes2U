using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers
{
    public class EstDeCouleurManager : GenericCRUDManager<Est_De_Couleur>, ICaracteristiquesRepository<Est_De_Couleur>
    {
        public EstDeCouleurManager(Clothes2UDbContext context) : base(context) { }

        public async Task<IEnumerable<Est_De_Couleur>> GetAllWithDetailsAsync()
        {
            return await BaseEstDeCouleurQuery().ToListAsync();
        }

        private IQueryable<Est_De_Couleur> BaseEstDeCouleurQuery()
        {
            return _context.Est_De_Couleurs
                .Include(a => a.Couleur)
                .Include(a => a.Annonce)
                .AsSplitQuery();
        }
    }
}
