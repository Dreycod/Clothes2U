using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Models.Conversation;
namespace FrontBlazor.Services;

public class ConversationService: WritableService<Conversation>
{
    protected readonly HttpClient _httpClient;
    public ConversationService(HttpClient httpClient) : base(httpClient)
    {
        _httpClient = httpClient;

    }

    public async Task<List<Conversation?>> GetConversationDetailById(int id)
    {
        return await _httpClient.GetFromJsonAsync<List<Conversation?>>(
       $"Conversation/conversation/{id}"
   );
    }
    public async Task<List<Conversation?>> GetConversationsByUserId(int id)
    {
        return await _httpClient.GetFromJsonAsync<List<Conversation?>>(
       $"Conversation/utilisateur/{id}"
   );
    }
}
