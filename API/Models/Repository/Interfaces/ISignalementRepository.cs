using API.Models.EntityFramework;

namespace API.Models.Repository
{
    public interface ISignalementRepository : IDataRepository<Signalement, int>
    {
        Task<IEnumerable<Signalement>> GetByUtilisateurAsync(int utilisateurId);
        Task<IEnumerable<Signalement>> GetByTypeAsync(int typeId);
        Task<Signalement> CreateWithRelationsAsync(Signalement signalement, int? annonceId, int? avisId, int? utilisateurSignaleId);
        Task DeleteSignalementByUserId(int id);
        Task<int> GetSignalementCount();
    }
}
