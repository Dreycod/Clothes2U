namespace FrontBlazor.Services.GenericIServices;
public interface ITailleService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
    Task<List<TEntity>> GetAllTailles();
}
