using API.Models.EntityFramework;

namespace API.Models.Repository
{
    public interface IDemandeRestaurationRepository<TEntity, TIdentifier>
    : IDataRepository<TEntity, TIdentifier>
    {
        Task<DemandeRestauration> GetActiveDemandeRestaurationByUserId(TIdentifier id);
    }
}
