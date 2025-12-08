namespace FrontBlazor.Services.GenericIServices;

public interface ICouleurService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
    Task<List<TEntity>> GetAllCouleurs();
}