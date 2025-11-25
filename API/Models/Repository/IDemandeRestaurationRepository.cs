using API.Models.EntityFramework;

namespace API.Models.Repository
{
    public interface IDemandeRestaurationRepository<TEntity, TIdentifier>
    : IDataRepository<TEntity, TIdentifier>
    {
        Task<IEnumerable<TEntity>> GetByUtilisateurId(TIdentifier utilisateurId);
        Task<IEnumerable<TEntity>> GetBySuspensionId(TIdentifier suspensionId);
    }
}
