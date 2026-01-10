namespace FrontBlazor.Services.Interfaces.GenericIServices;

public interface IWritableService<TEntity> where TEntity : class
{
    Task<TEntity?> AddAsync(TEntity entity);
    Task UpdateAsync(TEntity updatedEntity);
    Task DeleteAsync(int id);
}