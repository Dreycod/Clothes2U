namespace FrontBlazor.Services;

public interface IService<TEntity>  : IReadableService<TEntity>, IListableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
}