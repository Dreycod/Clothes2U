namespace FrontBlazor.Services;

public interface IListableService<TEntity> where TEntity : class
{
    Task<List<TEntity>?> GetAllAsync();
}