using Shared.DTO.Decision;

namespace FrontBlazor.Services.Interfaces;

public interface IDecisionService
{
    Task<List<DecisionDTO>> GetAllDecisionByModeratorIdAsync();
    Task<DecisionDetailDTO> GetDecisionDetailAsync(int DecisionId);
    Task<HttpResponseMessage> AddDecision(DecisionPostDTO decision);
}