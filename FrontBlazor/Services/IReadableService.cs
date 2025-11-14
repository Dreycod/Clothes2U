namespace FrontBlazor.Services;

public interface IReadableService<TEntity> where TEntity : class
{
    Task<TEntity> GetByIdAsync(int id);
}