using API.Models.EntityFramework;

namespace API.Models.Repository;

public interface IMotInterditRepository : IDataRepository<MotInterdit, int>
{
    Task<bool> EstInterdit(string mot);
}