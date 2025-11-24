namespace API.Models.Repository;

public interface INotificationRepository<TEntity> : IDataRepository<TEntity, int>
{
    public Task<IEnumerable<TEntity>> GetByUserId(int userId);
}