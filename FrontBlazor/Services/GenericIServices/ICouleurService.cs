namespace FrontBlazor.Services.GenericIServices;

interface ICouleurService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{

}