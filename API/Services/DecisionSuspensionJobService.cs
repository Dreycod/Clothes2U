using API.Models;
using API.Models.EntityFramework;
using API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class DecisionSuspensionJobService
    {
        private readonly Clothes2UDbContext _context;

        public DecisionSuspensionJobService(Clothes2UDbContext context)
        {
            _context = context;
        }

        public async Task VérifierSuspensionsExpirées()
        {
            var now = DateTime.UtcNow;

            var decisions = await _context.DecisionSuspensions
            .Include(ds => ds.UtilisateurSuspendu)
            .Where(ds =>
                !ds.EstTraitee &&
                ds.TypeSuspensionId == 1 &&
                ds.Sanctions.Any(s =>
                    s.Suspensions.Any(su => su.DateFinSuspension <= now)
                )
            )
            .ToListAsync();

            foreach (var s in decisions)
            {
                if (s.UtilisateurSuspendu != null)
                {
                    s.UtilisateurSuspendu.StatutId = 1; // réactive le compte
                }

                s.EstTraitee = true;
            }

            await _context.SaveChangesAsync();
        }
    }
}
