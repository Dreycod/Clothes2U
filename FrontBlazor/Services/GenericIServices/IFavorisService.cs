using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.Services.GenericIServices;

interface IFavorisService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{

}