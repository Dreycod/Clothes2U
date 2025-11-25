using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers
{
    public class DemandeRestaurationManager
    : GenericCRUDManager<DemandeRestauration>,
      IDemandeRestaurationRepository<DemandeRestauration, int>
    {
        public DemandeRestaurationManager(Clothes2UDbContext context) : base(context)
        {
        }

        private IQueryable<DemandeRestauration> BaseDemandeRestaurationQuery()
        {
            return _context.DemandesRestauration
                .Include(d => d.Plaignant)
                .Include(d => d.Suspension)
                .AsSplitQuery();
        }

        public override async Task<DemandeRestauration?> GetByIdAsync(int id)
        {
            return await BaseDemandeRestaurationQuery()
                .FirstOrDefaultAsync(d => d.DemandeRestaurationId == id);
        }

        public async Task<IEnumerable<DemandeRestauration>> GetByUtilisateurId(int utilisateurId)
        {
            return await BaseDemandeRestaurationQuery()
                .Where(d => d.UtilisateurId == utilisateurId)
                .ToListAsync();
        }

        public async Task<IEnumerable<DemandeRestauration>> GetBySuspensionId(int suspensionId)
        {
            return await BaseDemandeRestaurationQuery()
                .Where(d => d.SuspensionId == suspensionId)
                .ToListAsync();
        }
    }
}
