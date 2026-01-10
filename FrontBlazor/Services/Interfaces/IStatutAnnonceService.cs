using FrontBlazor.Services.Interfaces.GenericIServices;

namespace FrontBlazor.Services.Interfaces;

public interface IStatutAnnonceService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
}