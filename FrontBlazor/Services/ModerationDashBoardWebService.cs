using System.Net.Http.Json;
using Shared.DTO;

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
}