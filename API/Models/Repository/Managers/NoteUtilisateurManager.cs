using API.Models.EntityFramework;
using API.Models.Repository;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers
{
    public class NoteUtilisateurManager : GenericCRUDManager<NoteUtilisateur>, INoteUtilisateurRepository
    {
        public NoteUtilisateurManager(Clothes2UDbContext context) : base(context) { }

        private IQueryable<NoteUtilisateur> BaseNoteQuery()
        {
            return _context.NoteUtilisateurs
                .Include(n => n.Auteur)
                .Include(n => n.Cible)
                .AsSplitQuery();
        }

        public async Task<IEnumerable<NoteUtilisateur>> GetByUserIdAsync(int userId)
        {
            return await BaseNoteQuery()
                .Where(n => n.NoteId == userId)
                .ToListAsync();
        }

        public async Task<double> GetMoyenneNoteAsync(int userId)
        {
            return await BaseNoteQuery()
                .Where(n => n.NoteId == userId)
                .Select(n => (double)n.Note)
                .DefaultIfEmpty(0)
                .AverageAsync();
        }

        public override async Task<NoteUtilisateur?> GetByIdAsync(int id)
        {
            return await BaseNoteQuery()
                .FirstOrDefaultAsync(n => n.NoteUtilisateurId == id);
        }
    }
}
