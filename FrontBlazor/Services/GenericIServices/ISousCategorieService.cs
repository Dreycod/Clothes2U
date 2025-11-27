using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.Services.GenericIServices;

public interface ISousCategorieService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
}