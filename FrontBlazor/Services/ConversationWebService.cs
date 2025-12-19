using FrontBlazor.Services.GenericIServices;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Shared.DTO;
using Shared.DTO.Conversation;
using System.Net.Http.Json;

namespace FrontBlazor.Services;

public class ConversationWebService: WritableService<ConversationDTO>, IConversationService<ConversationDTO>
{
    public ConversationWebService(HttpClient httpClient) : base(httpClient) {}

    public Task<ConversationDTO> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ConversationDTO?> GetConversationDetailById(int id)
    {
        try
        {
            var response = await GetWithCredentialsAsync($"Conversation/conversation/{id}");
            response.EnsureSuccessStatusCode();
            
            var conversation = await response.Content.ReadFromJsonAsync<ConversationDTO?>();
            return conversation ?? new ConversationDTO();
        }
        catch
        {
            return null;
        }
        
    }
    public async Task<List<ConversationDTO?>> GetConversationsByUserId(int id)
    {
        try
        {
            var response = await GetWithCredentialsAsync($"Conversation/utilisateur/{id}");
            response.EnsureSuccessStatusCode();
            
            var conversations = await response.Content.ReadFromJsonAsync<List<ConversationDTO>?>();
            return conversations ?? new List<ConversationDTO?>();
        }
        catch
        {
            return null;
        }
    }

    public async Task<ConversationDTO> GetOrCreateConversation(int annonceId)
    {
        try
        {
            
            var response = await PostWithCredentialsAsync($"Conversation/annonce/{annonceId}", null);
            response.EnsureSuccessStatusCode();
            
            var conversation = await response.Content.ReadFromJsonAsync<ConversationDTO>();
            return conversation ?? new ConversationDTO();
        }
        catch
        {
            return null;
        }
    }
}
