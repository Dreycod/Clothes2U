using FrontBlazor.Services.Interfaces.GenericIServices;

namespace FrontBlazor.Services.Interfaces;

public interface IFavorisService<TEntity> : IWritableService<TEntity> where TEntity : class
{
    Task AddFavoris(int annonceId);
    Task DeleteFavoris(int annonceId);
}