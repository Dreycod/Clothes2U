using System.Net.Http.Json;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace FrontBlazor.Services;

public class ConversationWebService: WritableService<Conversation>, IConversationService<Conversation>
{
    public ConversationWebService(HttpClient httpClient) : base(httpClient) {}

    public Task<Conversation> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<Conversation?> GetConversationDetailById(int id)
    {
        try
        {
            // var request = new HttpRequestMessage(HttpMethod.Get, $"Conversation/conversation/{id}");
            // request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            //
            // var response = await _httpClient.SendAsync(request);
            //
            // if (!response.IsSuccessStatusCode)
            //     return null;
            //
            // return await _httpClient.GetFromJsonAsync<Conversation?>(
            //     $"Conversation/conversation/{id}");
            var response = await GetWithCredentialsAsync($"Conversation/conversation/{id}");
            response.EnsureSuccessStatusCode();
            
            var conversation = await response.Content.ReadFromJsonAsync<Conversation?>();
            return conversation ?? new Conversation();
        }
        catch
        {
            return null;
        }
        
    }
    public async Task<List<Conversation?>> GetConversationsByUserId(int id)
    {
        try
        {
            // var request = new HttpRequestMessage(HttpMethod.Get, $"Conversation/utilisateur/{id}");
            // request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            //
            // var response = await _httpClient.SendAsync(request);
            //
            // if (!response.IsSuccessStatusCode)
            //     return null;
            //
            // return await _httpClient.GetFromJsonAsync<List<Conversation?>>(
            //     $"Conversation/utilisateur/{id}"
            // );
            
            var response = await GetWithCredentialsAsync($"Conversation/utilisateur/{id}");
            response.EnsureSuccessStatusCode();
            
            var conversations = await response.Content.ReadFromJsonAsync<List<Conversation>?>();
            return conversations ?? new List<Conversation?>();
        }
        catch
        {
            return null;
        }
    }

    public async Task<Conversation> GetOrCreateConversation(int annonceId)
    {
        try
        {
            
            var response = await PostWithCredentialsAsync($"Conversation/annonce/{annonceId}", null);
            response.EnsureSuccessStatusCode();
            
            var conversation = await response.Content.ReadFromJsonAsync<Conversation>();
            return conversation ?? new Conversation();
        }
        catch
        {
            return null;
        }
    }
}
