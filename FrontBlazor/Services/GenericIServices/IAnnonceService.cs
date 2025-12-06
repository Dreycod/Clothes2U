using FrontBlazor.Models;

namespace FrontBlazor.Services.GenericIServices;

public interface IAnnonceService<TEntity>  : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
    Task<List<TEntity>> GetActiveAnnonces();
    Task<List<TEntity>?> GetAnnoncesByCategorieId(int id);
    Task<List<TEntity>?> GetAnnoncesBySousCategoryId(int id);
    Task<TEntity> GetAnnonceDetailById(int id);
    Task<List<TEntity>?> GetAnnoncesByUserIdAsync(int id);
    Task<List<TEntity>?> GetAnnonceByFilter(FilterDTO filterDto, int page = 1, int pageSize = 30);


}