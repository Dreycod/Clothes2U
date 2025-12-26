using Shared.DTO.Message;

namespace FrontBlazor.Services.GenericIServices;

public interface IConversationService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
    Task<List<TEntity>?> GetConversationsByUserId(int id);
    Task<TEntity?> GetConversationDetailById(int id);
    Task<MessageSignalementDTO>  GetMessageById(int id);
    
    Task<TEntity> GetOrCreateConversation(int annonceId);
}