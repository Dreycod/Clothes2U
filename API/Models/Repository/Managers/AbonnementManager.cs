using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers
{
    public class AbonnementManager : GenericCRUDManager<Abonnement>, IAbonnementRepository<Abonnement, int>
    {
        public AbonnementManager(Clothes2UDbContext context) : base(context)
        {
        }

        private IQueryable<Abonnement> BaseBloqueQuery()
        {
            return _context.Abonnements
                .Include(b => b.UtilisateurSuiveur)
                .Include(b => b.UtilisateurSuivis)
                .AsSplitQuery();
        }

        public override async Task<Abonnement?> GetByIdAsync(int id)
        {
            return await BaseBloqueQuery()
                .FirstOrDefaultAsync(b => b.AbonnementId == id);
        }

        public async Task<IEnumerable<Abonnement>> GetAbonnementById(int id)
        {
            return await BaseBloqueQuery()
                .Where(b => b.AbonnementId == id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Abonnement>> GetAllUtilisateurSuiviByFollower(int id)
        {
            return await BaseBloqueQuery()
                .Where(b => b.UtilisateurSuiveurId == id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Abonnement>> GetAllFollowersByUtilisateurSuivi(int id)
        {
            return await BaseBloqueQuery()
                .Where(b => b.UtilisateurSuivisId == id)
                .ToListAsync();
        }

        public async Task<bool> Exists(int suiveurId, int suiviId)
        {
            return await _context.Abonnements
                .AnyAsync(b => b.UtilisateurSuiveurId == suiveurId &&
                               b.UtilisateurSuivisId == suiviId);
        }

        public async Task<Abonnement> FindAbonnement(int suiveurId, int suiviId)
        {
            return _context.Abonnements.FirstOrDefault( a => a.UtilisateurSuiveurId == suiveurId && a.UtilisateurSuivisId == suiviId );
        }
    }
}
