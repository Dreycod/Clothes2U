
namespace FrontBlazor.Services.GenericIServices;
public interface ISignalementService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
}