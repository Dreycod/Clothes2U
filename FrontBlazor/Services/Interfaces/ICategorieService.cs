using Shared.DTO;

namespace FrontBlazor.Services.GenericIServices;

public interface ICategorieService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
    Task<List<TEntity>?> GetAllCategories();
    
}