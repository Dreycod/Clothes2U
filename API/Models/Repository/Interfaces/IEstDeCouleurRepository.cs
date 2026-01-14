namespace API.Models.Repository
{
    public interface IEstDeCouleurRepository<TEntity, TIdentifier> : IDataRepository<TEntity, TIdentifier>
    {
        Task<TEntity?> GetByEstDeCouleurId(TIdentifier id);
        Task<IEnumerable<TEntity>> GetAllWithDetailsAsync();
        Task<bool?> DeleteCouleurAnnonce(int annonceId);

    }
}
