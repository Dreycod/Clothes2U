using API.Models.EntityFramework;
using Shared.DTO.Decision;

namespace API.Services.Interfaces;
public interface IDecisionService
{
    Task<IEnumerable<DecisionDTO>> GetAllDecisionsByModeratorAsync(int moderatorId);
    Task<DecisionDetailDTO?> GetDecisionByIdAsync(int decisionId);
    Task<Decision> CreateDecisionAsync(DecisionPostDTO decisionDTO, int moderatorId);
}