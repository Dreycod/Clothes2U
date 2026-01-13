using Shared.DTO.Message;
using FrontBlazor.Services.Interfaces.GenericIServices;
using Shared.DTO;

namespace FrontBlazor.Services.Interfaces;

public interface IConversationService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
    Task<List<TEntity>?> GetConversationsByUserId(int id);
    Task<TEntity?> GetConversationDetailById(int id);
    Task<MessageSignalementDTO>  GetMessageById(int id);
    Task<List<StatutConversationDTO>> GetStatutConversation();
    
    Task<TEntity> GetOrCreateConversation(int annonceId);
}