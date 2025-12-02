using FrontBlazor.Models;

namespace FrontBlazor.Services.GenericIServices;

public interface ICategorieService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
    Task<List<TEntity>?> GetAllCategories();
    // keeping it because will probably have writable methods later ?
    // maybe brands too, sizes, so maybe a generic will exist for that.
    
}