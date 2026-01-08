using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.Services.GenericIServices;

public interface IFavorisService<TEntity> : IWritableService<TEntity> where TEntity : class
{
    Task AddFavoris(int annonceId);
    Task DeleteFavoris(int annonceId);
}