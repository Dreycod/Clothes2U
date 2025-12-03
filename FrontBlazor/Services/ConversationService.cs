using System.Net.Http.Json;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Models;
namespace FrontBlazor.Services;

public class ConversationService: WritableService<Conversation>, IConversationService<Conversation>
{
    public ConversationService(HttpClient httpClient) : base(httpClient) {}

    public Task<Conversation> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Conversation?>> GetConversationDetailById(int id)
    {
        return await _httpClient.GetFromJsonAsync<List<Conversation?>>(
       $"api/Conversation/conversation/{id}"
   );
    }
    public async Task<List<Conversation?>> GetConversationsByUserId(int id)
    {
        return await _httpClient.GetFromJsonAsync<List<Conversation?>>(
       $"api/Conversation/utilisateur/{id}"
   );
    }
}
