using Shared.DTO.Message;
using FrontBlazor.Services.Interfaces.GenericIServices;

namespace FrontBlazor.Services.Interfaces;

public interface IConversationService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
    Task<List<TEntity>?> GetConversationsByUserId(int id);
    Task<TEntity?> GetConversationDetailById(int id);
    Task<MessageSignalementDTO>  GetMessageById(int id);
    
    Task<TEntity> GetOrCreateConversation(int annonceId);
}