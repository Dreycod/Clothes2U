namespace FrontBlazor.Services.Interfaces.GenericIServices;

public interface IListableService<TEntity> where TEntity : class
{
    Task<List<TEntity>?> GetAllAsync();
}