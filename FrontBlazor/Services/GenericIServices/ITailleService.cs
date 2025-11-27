using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.Services.GenericIServices;
public interface ITailleService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
}
