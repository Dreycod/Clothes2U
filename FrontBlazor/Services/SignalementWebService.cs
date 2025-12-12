using System.Net.Http.Json;
using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.Services.Interfaces;

public class SignalementWebService : BaseGenericService, ISignalementService
{
    public SignalementWebService(HttpClient httpClient) : base(httpClient) { }
    public async Task<List<Signalement>> GetAllAsync()
    {
        var response = await GetWithCredentialsAsync("Signalement");
        response.EnsureSuccessStatusCode();
        var signalements = await response.Content.ReadFromJsonAsync<List<Signalement>>();
        return signalements ??  new List<Signalement>();
    }


    public async Task<List<Signalement>> GetAllByType(int typeId)
    {
        var response = await GetWithCredentialsAsync($"Signalement/Type/{typeId}");
        response.EnsureSuccessStatusCode();
        var signalements = await response.Content.ReadFromJsonAsync<List<Signalement>>();
        return signalements ??  new List<Signalement>();
    }

    public async Task<SignalementDetails> GetSignalementByIdAsync(int id)
    {
        var response = await GetWithCredentialsAsync($"Signalement/{id}");
        response.EnsureSuccessStatusCode();
        var signalement = await response.Content.ReadFromJsonAsync<SignalementDetails>();
        return signalement;
    }

    public Task<Signalement?> AddAsync(Signalement entity)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Signalement updatedEntity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int id)
    {
        throw new NotImplementedException();
    }
}