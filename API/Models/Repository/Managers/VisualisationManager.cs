using API.DTO.Visualisation;
using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers
{
    public class VisualisationManager : GenericCRUDManager<Visualisation>, IVisualisationRepository<Visualisation, int>
    {
        public VisualisationManager(Clothes2UDbContext context) : base(context) { }

        private IQueryable<Visualisation> BaseQuery()
        {
            return _context.Visualisations
                .Include(v => v.UtilisateurVisu)
                .Include(v => v.AnnonceVisu)
                .AsSplitQuery();
        }

        public async Task<bool> HasRecentView(int utilisateurId, int annonceId, TimeSpan delay)
        {
            DateTime limit = DateTime.UtcNow - delay;

            return await _context.Visualisations
                .AnyAsync(v =>
                    v.UtilisateurId == utilisateurId &&
                    v.AnnonceId == annonceId &&
                    (v.DateVisualisation + TimeSpan.FromMinutes(5)) >= limit);
        }

        public async Task<IEnumerable<Visualisation>> GetByUtilisateurId(int utilisateurId)
        {
            return await BaseQuery()
                .Where(v => v.UtilisateurId == utilisateurId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Visualisation>> GetByAnnonceId(int annonceId)
        {
            return await BaseQuery()
                .Where(v => v.AnnonceId == annonceId)
                .ToListAsync();
        }

        public async Task<int> CountViewsByAnnonce(int annonceId)
        {
            return await _context.Visualisations
                .CountAsync(v => v.AnnonceId == annonceId);
        }

        public async Task<IEnumerable<TopAnnonceStatsDTO>> GetTopAnnonces(int limit)
        {
            return await _context.Visualisations
                .GroupBy(v => v.AnnonceId)
                .Select(g => new TopAnnonceStatsDTO
                {
                    AnnonceId = g.Key,
                    NombreVues = g.Count()
                })
                .OrderByDescending(x => x.NombreVues)
                .Take(limit)
                .ToListAsync();
        }


    }
}
