using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers
{
    public class BloqueManager : GenericCRUDManager<Bloque>, IBloqueRepository<Bloque, int>
    {
        public BloqueManager(Clothes2UDbContext context) : base(context)
        {
        }

        private IQueryable<Bloque> BaseBloqueQuery()
        {
            return _context.Bloques
                .Include(b => b.UtilisateurBloqueur)
                .Include(b => b.UtilisateurBloque)
                .AsSplitQuery();
        }

        public override async Task<Bloque?> GetByIdAsync(int id)
        {
            return await BaseBloqueQuery()
                .FirstOrDefaultAsync(b => b.BloqueId == id);
        }

        public async Task<IEnumerable<Bloque>> GetByUtilisateurBloquantId(int id)
        {
            return await BaseBloqueQuery()
                .Where(b => b.UtilisateurBloqueurId == id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Bloque>> GetByUtilisateurBloqueId(int id)
        {
            return await BaseBloqueQuery()
                .Where(b => b.UtilisateurBloqueId == id)
                .ToListAsync();
        }
        public async Task<bool> Exists(int bloqueurId, int bloqueId)
        {
            return await _context.Bloques
                .AnyAsync(b => b.UtilisateurBloqueurId == bloqueurId &&
                               b.UtilisateurBloqueId == bloqueId);
        }
        public async Task<Bloque?> GetIfExists(int bloqueurId, int bloqueId)
        {
            return await _context.Bloques
                .FirstOrDefaultAsync(b => b.UtilisateurBloqueurId == bloqueurId &&
                                          b.UtilisateurBloqueId == bloqueId);
        }
    }
}
