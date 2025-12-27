using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public interface IDecisionRepository : IDataRepository<Decision, int>
{
    Task<IEnumerable<Decision>> GetAllDecisionsByModerateurId(int id);
    Task<IEnumerable<Decision>> GetAllDecisionByUserId(int id);
    Task<IEnumerable<Decision>> GetAllActiveDecisionByUserId(int id);
    Task<Decision> AddAsync(Decision decision);
}