using System.Net.Http.Json;
using System.Text.Json;
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
    public async Task<SignalementCreate> CreateSignalement(SignalementCreate signalement)
    {

        Console.WriteLine(
            JsonSerializer.Serialize(signalement)
        );

        var response = await PostWithCredentialsAsync("Signalement", JsonContent.Create(signalement));
        response.EnsureSuccessStatusCode();
        var createdSignalement = await response.Content.ReadFromJsonAsync<SignalementCreate>();
        return createdSignalement;
    }

    public async Task<Signalement?> AddAsync(Signalement entity)
    {
        await PostWithCredentialsAsync("Signalement", JsonContent.Create(entity));
        return entity;
    }

    public Task UpdateAsync(Signalement updatedEntity)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(int id)
    {
        await DeleteWithCredentialsAsync($"Signalement/{id}");
    }
}