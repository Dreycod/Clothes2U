namespace FrontBlazor.Services.GenericIServices;

public interface IAnnonceService<TEntity>  : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
    Task<List<TEntity>?> GetActiveAnnonces();
    Task<List<TEntity>?> GetAnnoncesByCategorieId(int id);
    Task<List<TEntity>?> GetAnnoncesBySousCategoryId(int id);
    Task<List<TEntity>?> GetAnnonceDetailById(int id);
    Task<List<TEntity>?> GetAnnoncesByUserIdAsync(int id);
}