using API.Models.EntityFramework;

namespace API.Models.Repository;

public interface IConversationRepository<TEntity, TIdentifier> : IDataRepository<TEntity, TIdentifier>, ISuspendRepository
{
    Task<TEntity> GetByIdAsync(TIdentifier id);
    Task<IEnumerable<TEntity>> GetAllAsyncByUser(TIdentifier id);
    Task<int?> GetOtherUser(int currentUserId, TEntity entity);
    Task ChangeAllStatutConversation(int annonceId, int conversationId, int statutId, int conversationStatutId);
    Task<Conversation?> GetByUserAndAnnonceAsync(int userId, int annonceId);

}