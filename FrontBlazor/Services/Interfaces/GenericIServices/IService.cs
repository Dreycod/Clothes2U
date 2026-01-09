namespace FrontBlazor.Services.Interfaces.GenericIServices;

public interface IService<TEntity, CreateTEntity>  : IReadableService<TEntity>, IListableService<TEntity>, IWritableService<TEntity, CreateTEntity> where TEntity : class where CreateTEntity : class
{
}