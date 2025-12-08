using FrontBlazor.Models;

namespace FrontBlazor.Services.GenericIServices;

public interface IMessageService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
    Task<List<TEntity>?> GetMessagesByConversationId(int id);
    Task<List<TEntity>?> GetMessagesByUserId(int id);
    Task<HttpResponseMessage> PostMessageTexte(Message message);

}