namespace FrontBlazor.Services.GenericIServices;

public interface IStatutAnnonceService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
}