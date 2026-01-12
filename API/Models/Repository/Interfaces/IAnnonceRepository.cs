using API.Models.EntityFramework;
using Shared.DTO.Annonce;

namespace API.Models.Repository;

public interface IAnnonceRepository<TEntity, TIdentifier, TFilterEntity> : IDataRepository<TEntity, TIdentifier>, ISuspendRepository
{
    Task<IEnumerable<TEntity>> GetActiveAnnonces();
    Task<IEnumerable<TEntity>> GetByUtilisateurId(TIdentifier id);
    
    Task<IEnumerable<TEntity>> GetByUtilisateurFavoris(TIdentifier id);
    Task<IEnumerable<TEntity>> FilterAsync(TFilterEntity filterDto, int page, int pageSize, int? currentUserId = null);
    Task<IEnumerable<TEntity>> GetSimilarAsync(TIdentifier annonceId, int page, int pageSize, int? currentUserId = null);
    Task<IEnumerable<TEntity>> GetByIdsAsync(IEnumerable<int> ids);
}