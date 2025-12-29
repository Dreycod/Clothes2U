using System.Net.Http.Json;
using System.Text.Json;
using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;
using Shared.DTO.Signalement;

namespace FrontBlazor.Services.Interfaces;

public class SignalementWebService : BaseGenericService, ISignalementService
{
    public SignalementWebService(HttpClient httpClient) : base(httpClient) { }
    public async Task<List<SignalementDTO>> GetAllAsync()
    {
        var response = await GetWithCredentialsAsync("Signalement");
        response.EnsureSuccessStatusCode();
        var signalements = await response.Content.ReadFromJsonAsync<List<SignalementDTO>>();
        return signalements ??  new List<SignalementDTO>();
    }


    public async Task<List<SignalementDTO>> GetAllByType(int typeId)
    {
        var response = await GetWithCredentialsAsync($"Signalement/Type/{typeId}");
        response.EnsureSuccessStatusCode();
        var signalements = await response.Content.ReadFromJsonAsync<List<SignalementDTO>>();
        return signalements ??  new List<SignalementDTO>();
    }

    public async Task<SignalementDetailsDTO> GetSignalementByIdAsync(int id)
    {
        var response = await GetWithCredentialsAsync($"Signalement/{id}");
        response.EnsureSuccessStatusCode();
        var signalement = await response.Content.ReadFromJsonAsync<SignalementDetailsDTO>();
        return signalement;
    }
    public async Task<SignalementDetailsDTO> CreateSignalement(SignalementCreateDTO signalement)
    {
        var response = await PostWithCredentialsAsync("Signalement", JsonContent.Create(signalement));
        response.EnsureSuccessStatusCode();
        var createdSignalement = await response.Content.ReadFromJsonAsync<SignalementDetailsDTO>();
        return createdSignalement;
    }

    public async Task<SignalementDTO?> AddAsync(SignalementDTO entity)
    {
        await PostWithCredentialsAsync("Signalement", JsonContent.Create(entity));
        return entity;
    }

    public Task UpdateAsync(SignalementDTO updatedEntity)
    {
        throw new NotImplementedException();
    }

    public async Task DeleteAsync(int id)
    {
        await DeleteWithCredentialsAsync($"Signalement/Delete/{id}");
    }
}