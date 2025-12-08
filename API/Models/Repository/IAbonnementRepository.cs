using API.DTO.Abonnement;

namespace API.Models.Repository
{
    public interface IAbonnementRepository<TEntity, TIdentifier> : IDataRepository<TEntity, TIdentifier>
    {
        Task<IEnumerable<TEntity>> GetAbonnementById(TIdentifier id);
        Task<IEnumerable<TEntity>> GetAllUtilisateurSuiviByFollower(TIdentifier id);
        Task<IEnumerable<TEntity>> GetAllFollowersByUtilisateurSuivi(TIdentifier id);

        Task<bool> Exists(int suiveurId, int suiviId);
        Task<TEntity> FindAbonnement(int suiveurId, int suiviId);
    }
}
