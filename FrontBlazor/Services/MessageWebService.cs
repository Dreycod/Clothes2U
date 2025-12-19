using Shared.DTO;
using Shared.DTO.Message;
using FrontBlazor.Services.GenericIServices;
using System.Net.Http;
using System.Net.Http.Json;

namespace FrontBlazor.Services;

public class MessageWebService : WritableService<MessageDTO>, IMessageService<MessageDTO>
{
    public MessageWebService(HttpClient httpClient) : base(httpClient) { }

    public Task<MessageDTO> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<MessageDTO>?> GetMessagesByConversationId(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<MessageDTO?>> GetMessagesByUserId(int id)
    {
        return await _httpClient.GetFromJsonAsync<List<MessageDTO?>>(
       $"Message/utilisateur/{id}");
    }
    public async Task<HttpResponseMessage> PostMessageTexte(MessageDTO message)
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