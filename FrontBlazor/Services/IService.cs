namespace FrontBlazor.Services;

public interface IService<TEntity> where TEntity : class
{
    Task<List<TEntity>?> GetAllAsync();
    Task<TEntity?> GetByIdAsync(int id);
    Task<TEntity?> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity updatedEntity);
    Task DeleteAsync(int id);
}