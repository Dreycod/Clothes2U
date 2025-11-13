namespace API.Models.Repository.Managers;

public interface IFiltrableByIdRepository<TEntity, TIdentifier>
{
    Task<IEnumerable<TEntity>> GetAllAsyncByIdentifier(TIdentifier id);
}