namespace API.Models.Repository;

public interface IWritableRepository<in TEntity>
{
    Task AddAsync(TEntity entity);
    Task UpdateAsync(TEntity entityToUpdate, TEntity entity);
    Task DeleteAsync(TEntity entity);
}