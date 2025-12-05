namespace API.Models.Repository;

public interface IConversationRepository<TEntity, TIdentifier> : IDataRepository<TEntity, TIdentifier>
{
    Task<TEntity> GetByIdAsync(TIdentifier id);
    Task<IEnumerable<TEntity>> GetAllAsyncByUser(TIdentifier id);
    Task<int?> GetOtherUser(int currentUserId, TEntity entity);

}