using API.Models.EntityFramework;

namespace API.Models.Repository;

public interface IMotInterditRepository : IDataRepository<MotInterdit, int>
{
    Task<List<string>> GetAllLibellesAsync();
    Task<bool> Exists(string libelleMot);
}