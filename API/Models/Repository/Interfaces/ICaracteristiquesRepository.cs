using API.Models.EntityFramework;
namespace API.Models.Repository
{
    public interface ICaracteristiquesRepository<TEntity> : IDataRepository<TEntity, int> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllWithDetailsAsync();
    }
}
