using API.Models.EntityFramework;
namespace API.Models.Repository
{
    public interface INoteUtilisateurRepository : IDataRepository<NoteUtilisateur, int>, ISuspendRepository
    {
        Task<IEnumerable<NoteUtilisateur>> GetByUserIdAsync(int userId, int page, int pageSize);
        Task<double> GetMoyenneNoteAsync(int userId);
        Task<NoteUtilisateur?> GetNoteByUserIdAndOtherUserId(int userId, int otherUserId);
    }
}
