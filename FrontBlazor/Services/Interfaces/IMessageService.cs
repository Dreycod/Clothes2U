using Shared.DTO;
using Shared.DTO.Message;

namespace FrontBlazor.Services.GenericIServices;

public interface IMessageService : IReadableService<MessageTextDTO>, IWritableService<MessageTextDTO>
{
    Task<List<MessageTextDTO>?> GetMessagesByConversationId(int id);
    Task<List<MessageTextDTO>?> GetMessagesByUserId(int id);
    Task<HttpResponseMessage> PostMessageTexte(MessageTextePostDTO message);
    Task<HttpResponseMessage> PostMessageDemande(MessageDemandePostDTO message);
    Task<HttpResponseMessage> PostMessagePayee(MessageEstPayeePostDTO message);
    Task<HttpResponseMessage> PostMessageEnvoieColis(MessageEnvoisColisPostDTO message);
    Task<HttpResponseMessage> CancelMessagePayee(int messageId);
    Task<MessageDTO> GetLastMessageByConversationId(int id);
    Task MaskAsRead(int messageId);
    Task AnswerPriceProposal(int messageId, bool accepted);
    //Task DeclinePriceProposal(int messageId);
}