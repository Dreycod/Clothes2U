namespace FrontBlazor.Services.GenericIServices;

public interface IMarqueService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
    Task<List<TEntity>> GetAllMarques();
}
