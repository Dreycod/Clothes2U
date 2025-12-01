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

            var suspensions = await _context.DecisionSuspensions
                .Include(s => s.UtilisateurSuspendu)
                .Where(s => !s.EstTraitee && s.DateFinSuspension <= now && s.TypeSuspensionId == 1)
                .ToListAsync();

            foreach (var s in suspensions)
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
