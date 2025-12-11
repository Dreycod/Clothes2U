namespace FrontBlazor.Services.GenericIServices;

public interface IAbonnementService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
    Task AddAbonnement(int utilisateurId);
    Task DeleteAbonnement(int utilisateurId);
}
