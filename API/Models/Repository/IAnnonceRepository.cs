namespace API.Models.Repository;

public interface IAnnonceRepository<TEntity, TIdentifier> : IDataRepository<TEntity, TIdentifier>
{
    
    Task<IEnumerable<TEntity>> GetByCategorieId(TIdentifier id);
    Task<IEnumerable<TEntity>> GetBySousCategorieId(TIdentifier id);
    Task<IEnumerable<TEntity>> GetActiveAnnonces();
    
}