namespace FrontBlazor.Services.Interfaces.GenericIServices;

public interface IService<TEntity>  : IReadableService<TEntity>, IListableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
}