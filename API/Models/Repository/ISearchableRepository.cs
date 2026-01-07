namespace API.Models.Repository;

public interface ISearchableRepository<T>
{
    Task<IEnumerable<T>> GetByString(string filter);
}