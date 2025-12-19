using Shared.DTO.Bloque;

namespace API.Models.Repository
{
    public interface IBloqueRepository<TEntity, TIdentifier> : IDataRepository<TEntity, TIdentifier>
    {
        Task<IEnumerable<TEntity>> GetByUtilisateurBloquantId(TIdentifier id);
        Task<IEnumerable<TEntity>> GetByUtilisateurBloqueId(TIdentifier id);
        Task<bool> Exists(int bloqueurId, int bloqueId);
        Task<TEntity?> GetIfExists(int bloqueurId, int bloqueId);
    }
}
