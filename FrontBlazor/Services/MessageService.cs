using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;
using System.Net.Http;
using System.Net.Http.Json;

namespace FrontBlazor.Services;

public class MessageService : WritableService<Message>, IMessageService<Message>
{
    public MessageService(HttpClient httpClient) : base(httpClient) { }

    public Task<Message> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<Message>?> GetMessagesByConversationId(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Message?>> GetMessagesByUserId(int id)
    {
        return await _httpClient.GetFromJsonAsync<List<Message?>>(
       $"Message/utilisateur/{id}");
    }
    public async Task<HttpResponseMessage> PostMessageTexte(Message message)
    {
        var body = JsonContent.Create(message);
        return await PostWithCredentialsAsync($"Message/texte/",body);
    }
}