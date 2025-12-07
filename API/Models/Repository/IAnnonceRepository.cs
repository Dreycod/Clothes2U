using API.DTO.Annonce;

namespace API.Models.Repository;

public interface IAnnonceRepository<TEntity, TIdentifier, TFilterEntity> : IDataRepository<TEntity, TIdentifier>
{
    Task<IEnumerable<TEntity>> GetActiveAnnonces();

    Task<IEnumerable<TEntity>> GetByUtilisateurId(TIdentifier id);
    
    Task<IEnumerable<TEntity>> GetByUtilisateurFavoris(TIdentifier id, int page, int pageSize);
    Task<IEnumerable<TEntity>> FilterAsync(TFilterEntity filterDto, int page, int pageSize);
}