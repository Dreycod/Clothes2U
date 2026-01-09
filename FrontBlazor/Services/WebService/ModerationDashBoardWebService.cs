using System.Net.Http.Json;
using Shared.DTO;
using FrontBlazor.Services.GenericService;

namespace FrontBlazor.Services.Interfaces;

public class ModerationDashBoardWebService : BaseGenericService, IModerationDashboardService
{
    public ModerationDashBoardWebService(HttpClient httpClient) : base(httpClient) { }

    public async Task<DashBoardStatistics> GetStatistics()
    {
        var response = await GetWithCredentialsAsync("Moderateur/Statistics");
        response.EnsureSuccessStatusCode();
        var statistics = await response.Content.ReadFromJsonAsync<DashBoardStatistics>();
        return statistics;
    }

    public async Task<List<ActivityDTO>> GetActivity()
    {
        var response = await GetWithCredentialsAsync("Moderateur/Activity");
        response.EnsureSuccessStatusCode();
        var activity = await response.Content.ReadFromJsonAsync<List<ActivityDTO>>();
        return activity;
    }
}