using API.Models.EntityFramework;

namespace API.Models.Repository
{
    public interface ISignalementRepository : IDataRepository<Signalement, int>
    {
        Task<IEnumerable<Signalement>> GetByUtilisateurAsync(int utilisateurId);
        Task<IEnumerable<Signalement>> GetByTypeAsync(int typeId);

        Task<IEnumerable<Signalement>> GetSignalementAnnonceAsync(int annonceId);
        Task<IEnumerable<Signalement>> GetSignalementAvisAsync(int avisId);
        Task<IEnumerable<Signalement>> GetSignalementUtilisateurAsync(int utilisateurSignaleId);
        Task<Signalement> CreateWithRelationsAsync(Signalement signalement, int? annonceId, int? avisId, int? utilisateurSignaleId);

    }
}
