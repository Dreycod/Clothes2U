using FrontBlazor.Services.Interfaces.GenericIServices;

namespace FrontBlazor.Services;

public interface ICouleurService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
}