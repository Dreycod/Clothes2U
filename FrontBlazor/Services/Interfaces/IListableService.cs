namespace FrontBlazor.Services.GenericIServices;

public interface IListableService<TEntity> where TEntity : class
{
    Task<List<TEntity>?> GetAllAsync();
    Task<List<TEntity>?> GetAllWithDetailsAsync();
}