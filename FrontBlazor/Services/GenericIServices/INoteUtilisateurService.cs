namespace FrontBlazor.Services.GenericIServices;

public interface INoteUtilisateurService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{

}