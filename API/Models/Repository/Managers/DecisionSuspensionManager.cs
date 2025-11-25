using API.DTO.Decision_suspension;
using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers
{
    public class DecisionSuspensionManager
       : GenericCRUDManager<Decision_suspension>,
         IDecisionSuspensionRepository<Decision_suspension, int>
    {
        public DecisionSuspensionManager(Clothes2UDbContext context)
            : base(context)
        {
        }

        // Base Query avec tous les Includes nécessaires
        private IQueryable<Decision_suspension> BaseDecisionSuspensionQuery()
        {
            return _context.DecisionSuspensions
                .Include(d => d.UtilisateurSuspendu)
                .Include(d => d.Decisionnaire)
                .Include(d => d.AnnonceSuspendu)
                .AsSplitQuery();
        }

        // GET BY ID 
        public override async Task<Decision_suspension?> GetByIdAsync(int id)
        {
            return await BaseDecisionSuspensionQuery()
                .FirstOrDefaultAsync(d => d.Decision_suspensionId == id);
        }

        // Toutes les suspensions d’un utilisateur
        public async Task<IEnumerable<Decision_suspension>> GetAllAsyncByUser(int utilisateurId)
        {
            return await BaseDecisionSuspensionQuery()
                .Where(d => d.UtilisateurId == utilisateurId)
                .ToListAsync();
        }

        // Suspensions encore actives
        public async Task<IEnumerable<Decision_suspension>> GetActiveSuspensionsAsync()
        {
            return await BaseDecisionSuspensionQuery()
                .Where(d => d.DateFinSuspension > DateTime.UtcNow)
                .ToListAsync();
        }

        // Recherche filtrée
        public async Task<IEnumerable<Decision_suspension>> SearchAsync(DecisionSuspensionSearchRequestDTO request)
        {
            IQueryable<Decision_suspension> query = BaseDecisionSuspensionQuery();

            if (request.UtilisateurId.HasValue)
                query = query.Where(d => d.UtilisateurId == request.UtilisateurId.Value);

            if (request.UtilisateurAdminId.HasValue)
                query = query.Where(d => d.UtilisateurAdminId == request.UtilisateurAdminId.Value);

            if (request.AnnonceId.HasValue)
                query = query.Where(d => d.AnnonceId == request.AnnonceId.Value);

            if (request.DateDebut.HasValue)
                query = query.Where(d => d.DateDebutSuspension >= request.DateDebut.Value);

            if (request.DateFin.HasValue)
                query = query.Where(d => d.DateFinSuspension <= request.DateFin.Value);

            return await query.ToListAsync();
        }
    }
}
