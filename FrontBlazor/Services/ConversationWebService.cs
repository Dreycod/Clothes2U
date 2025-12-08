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
            var request = new HttpRequestMessage(HttpMethod.Get, "Login/me");
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            return await _httpClient.GetFromJsonAsync<Conversation?>(
                $"Conversation/conversation/{id}");
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
            var request = new HttpRequestMessage(HttpMethod.Get, "Login/me");
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            return await _httpClient.GetFromJsonAsync<List<Conversation?>>(
                $"Conversation/utilisateur/{id}"
            );
        }
        catch
        {
            return null;
        }
        
    }
}
