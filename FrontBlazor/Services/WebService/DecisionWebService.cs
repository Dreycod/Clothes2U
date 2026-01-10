using System.Net.Http.Json;
using System.Text.Json;
using FrontBlazor.Services.GenericService;
using FrontBlazor.Services.Interfaces;
using Shared.DTO.Decision;

namespace FrontBlazor.Services;

public class DecisionWebService : BaseGenericService, IDecisionService
{
    public DecisionWebService(HttpClient httpClient) : base(httpClient){}
    public async Task<List<DecisionDTO>> GetAllDecisionByModeratorIdAsync()
    {
        var response = await GetWithCredentialsAsync("Decision");
        response.EnsureSuccessStatusCode();
        var sanctions = await response.Content.ReadFromJsonAsync<List<DecisionDTO>>();
        return sanctions;
    }

    public async Task<DecisionDetailDTO> GetDecisionDetailAsync(int DecisionId)
    {
        var response = await GetWithCredentialsAsync($"Decision/{DecisionId}");
        response.EnsureSuccessStatusCode();
        var sanction = await response.Content.ReadFromJsonAsync<DecisionDetailDTO>();
        return sanction;
    }

    public async Task<HttpResponseMessage> AddDecision(DecisionPostDTO decision)
    {
        var body = JsonContent.Create(decision);
        return await PostWithCredentialsAsync("Decision", body);
    }
}