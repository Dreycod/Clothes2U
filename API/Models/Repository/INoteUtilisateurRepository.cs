using API.Models.EntityFramework;
namespace API.Models.Repository
{
    public interface INoteUtilisateurRepository : IDataRepository<NoteUtilisateur, int>
    {
        Task<IEnumerable<NoteUtilisateur>> GetByUserIdAsync(int userId);
        Task<double> GetMoyenneNoteAsync(int userId);
    }
}
