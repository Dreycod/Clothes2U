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

        public async Task<IEnumerable<NoteUtilisateur>> GetByUserIdAsync(int userId, int page, int pageSize)
        {
            IQueryable<NoteUtilisateur> query = BaseNoteQuery()
                .Where(n => n.CibleId == userId)
                .OrderByDescending(n => n.DatePublication);
    
            // Pagination
            int skip = (page - 1) * pageSize;
            var result = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
    
            return result;
        }

        public async Task<double> GetMoyenneNoteAsync(int userId)
        {
            var notes = await _context.NoteUtilisateurs
                .Where(n => n.CibleId == userId)
                .Select(n => (double)n.Note)
                .ToListAsync();

            return notes.Count == 0 ? 0 : notes.Average();
        }


        public override async Task<NoteUtilisateur?> GetByIdAsync(int id)
        {
            return await BaseNoteQuery()
                .FirstOrDefaultAsync(n => n.NoteUtilisateurId == id);
        }
        public async Task SuspendElement(int id)
        {
            NoteUtilisateur noteUtilisateur = _context.NoteUtilisateurs.Find(id);
            noteUtilisateur.Statut = false;
            _context.NoteUtilisateurs.Update(noteUtilisateur);
            await _context.SaveChangesAsync();
        }
    }
}
