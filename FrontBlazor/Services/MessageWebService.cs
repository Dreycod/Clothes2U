using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;
using System.Net.Http;
using System.Net.Http.Json;

namespace FrontBlazor.Services;

public class MessageWebService : WritableService<Message>, IMessageService<Message>
{
    public MessageWebService(HttpClient httpClient) : base(httpClient) { }

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
        return await PostWithCredentialsAsync($"Message/texte",body);
    }

    public async Task MaskAsRead(int messageId)
    {
        var response = await _httpClient.PutAsync($"Message/markAsRead/{messageId}", null);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"❌ Failed to mark message {messageId} as read. Status: {response.StatusCode}");
        }
    }

}