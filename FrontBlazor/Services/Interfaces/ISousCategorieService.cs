using FrontBlazor.Services.Interfaces.GenericIServices;

namespace FrontBlazor.Services.Interfaces;

public interface ISousCategorieService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
    Task<List<TEntity>?> GetSousCategorie(int Categorieid);
}