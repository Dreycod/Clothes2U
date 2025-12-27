using System.Net.Http.Json;
using System.Text.Json;
using FrontBlazor.Services.Interfaces;
using Shared.DTO.Decision;

namespace FrontBlazor.Services;

public class DecisionWebService : BaseGenericService, IDecisionService
{
    public DecisionWebService(HttpClient httpClient) : base(httpClient){}
    public async Task<DecisionDTO> GetAllDecisionByModeratorIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<DecisionDetailDTO> GetDecisionDetailAsync(int DecisionId)
    {
        throw new NotImplementedException();
    }

    public async Task<HttpResponseMessage> AddDecision(DecisionPostDTO decision)
    {
        var body = JsonContent.Create(decision);
        return await PostWithCredentialsAsync("Decision", body);
    }
}