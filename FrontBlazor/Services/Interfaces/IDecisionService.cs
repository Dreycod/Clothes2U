using Shared.DTO.Decision;

namespace FrontBlazor.Services.Interfaces;

public interface IDecisionService
{
    Task<DecisionDTO> GetAllDecisionByModeratorIdAsync(int id);
    Task<DecisionDetailDTO> GetDecisionDetailAsync(int DecisionId);
    Task<HttpResponseMessage> AddDecision(DecisionPostDTO decision);
}