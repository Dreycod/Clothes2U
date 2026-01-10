using System.Net.Http.Json;
using FrontBlazor.Services.GenericService;
using FrontBlazor.Services.Interfaces;
using Shared.DTO.Marque;
using Stripe;

namespace FrontBlazor.Services;

public class MarqueWebService : CaracteristiqueService<MarqueDTO>, IMarqueService
{
    public MarqueWebService(HttpClient httpClient) : base(httpClient){}
    public async Task<List<MarqueDTO>> SearchAsync(string? query)
    {
        string url = string.IsNullOrWhiteSpace(query)
            ? "Marque/byName"
            : $"Marque/byName?name={Uri.EscapeDataString(query)}";

        var result = await _httpClient.GetFromJsonAsync<List<MarqueDTO>>(url);

        return result;
    }
}