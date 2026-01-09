using Shared.DTO;
using Shared.DTO.Message;
using FrontBlazor.Services.GenericIServices;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Sockets;
using Shared.DTO.Conversation;

namespace FrontBlazor.Services;

public class MessageWebService : WritableService<MessageTextDTO>, IMessageService
{
    public MessageWebService(HttpClient httpClient) : base(httpClient) { }

    public Task<MessageTextDTO> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<MessageTextDTO>?> GetMessagesByConversationId(int id)
    {
        throw new NotImplementedException();
        //return await _httpClient.GetFromJsonAsync<MessageDTO>($"Message/{id}");
    }

    public async Task<List<MessageTextDTO?>> GetMessagesByUserId(int id)
    {
        return await _httpClient.GetFromJsonAsync<List<MessageTextDTO?>>(
            $"Message/utilisateur/{id}");
    }
    public async Task<HttpResponseMessage> PostMessageTexte(MessageTextePostDTO message)
    {
        var body = JsonContent.Create(message);
        return await PostWithCredentialsAsync($"Message/texte",body);
    }
    
    public async Task<HttpResponseMessage> PostMessageDemande(MessageDemandePostDTO message)
    {
        var body = JsonContent.Create(message);
        return await PostWithCredentialsAsync($"Message/demande",body);
    }

    public async Task<HttpResponseMessage> PostMessagePayee(MessageEstPayeePostDTO message)
    {
        var body = JsonContent.Create(message);
        return await PostWithCredentialsAsync($"Message/payee",body);
    }
    
    public async Task<HttpResponseMessage> PostMessageEnvoieColis(MessageEnvoisColisPostDTO message)
    {
        var body = JsonContent.Create(message);
        return await PostWithCredentialsAsync($"Message/envoieColis",body);
    }

    public async Task<HttpResponseMessage> CancelMessagePayee(int messageId)
    {
        return await PutWithCredentialsAsync($"Message/annulePayement/{messageId}", null);
    }

    public async Task MaskAsRead(int messageId)
    {
        var response = await _httpClient.PutAsync($"Message/markAsRead/{messageId}", null);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"❌ Failed to mark message {messageId} as read. Status: {response.StatusCode}");
        }
    }

    public async Task AnswerPriceProposal(int messageId, bool accepted)
    {
        var response = await _httpClient.PutAsync($"Message/Answer/{messageId}/{accepted}", null);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"❌ Failed to mark message {messageId} as read. Status: {response.StatusCode}");
        }
    }

    public async Task<MessageDTO> GetLastMessageByConversationId(int id)
    {
        var conversationDto = await _httpClient.GetFromJsonAsync<ConversationDTO>($"Conversation/conversation/{id}");
        return conversationDto.ListMessages.LastOrDefault();
    }

}