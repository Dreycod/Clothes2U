using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers
{
    public class EstDeCouleurManager : GenericCRUDManager<Est_De_Couleur>, IEstDeCouleurRepository<Est_De_Couleur, int>
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
        public async Task<Est_De_Couleur?> GetByEstDeCouleurId(int id)
        {
            return await BaseEstDeCouleurQuery()
                .FirstOrDefaultAsync(e => e.EstDeCouleurId == id);
        }
        public async Task<bool?> DeleteCouleurAnnonce(int annonceId)
        {
            var couleursToDelete = _context.Est_De_Couleurs
                .Where(e => e.AnnonceId == annonceId);
            if (!couleursToDelete.Any())
            {
                return null; 
            }
            _context.Est_De_Couleurs.RemoveRange(couleursToDelete);
            await _context.SaveChangesAsync();
            return true; 
        }
    }
}
