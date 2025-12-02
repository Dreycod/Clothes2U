namespace FrontBlazor.Services.GenericIServices;

public interface IConversationService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
    Task<List<TEntity>?> GetConversationsByUserId(int id);
    Task<List<TEntity>?> GetConversationDetailById(int id);
}