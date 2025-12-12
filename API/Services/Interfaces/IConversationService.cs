using API.Models.EntityFramework;

namespace API.Services;

public interface IConversationService
{
    Task<Conversation> GetOrCreateConversation(int annonceId, int userId);
}