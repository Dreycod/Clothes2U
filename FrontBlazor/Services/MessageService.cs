using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;
using System.Net.Http;

namespace FrontBlazor.Services;

public class MessageService : WritableService<Message>, IMessageService<Message>
{
    protected readonly HttpClient _httpClient;
    public MessageService(HttpClient httpClient) : base(httpClient)
    {
        _httpClient = httpClient;

    }

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
       $"Message/utilisateur/{id}"
    );
    }
}