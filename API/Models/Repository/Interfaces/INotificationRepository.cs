using API.Models.EntityFramework;

namespace API.Models.Repository;

public interface INotificationRepository : IDataRepository<Notification, int>
{
    public Task<IEnumerable<Notification>> GetByUserId(int userId);
    public Task<int> GetNotificationsUnreadCountByUserId(int userId);
    public Task MarkAsRead(int userId);
    public Task CreateNotificationAvertissement(NotificationAvertissement notification);
    public Task DeleteMessageNotificationByConversationId(int id, int userId);
    Task DeleteNotificationAnnonceForUser(int annonceId, int userId);
}

