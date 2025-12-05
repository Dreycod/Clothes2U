using API.Models.EntityFramework;

namespace API.Models.Repository;

public interface INotificationRepository : IDataRepository<Notification, int>
{
    public Task<IEnumerable<Notification>> GetByUserId(int userId);
    public Task<int> GetNotificationsUnreadCountByUserId(int userId);
    public Task MarkAsRead(int userId);
}

public interface INotificationMessageRepository : IDataRepository<NotificationMessage, int>
{
}

public interface INotificationNouvelleAnnonceRepository : IDataRepository<NotificationNouvelleAnnonce, int>
{
}

public interface INotificationModificationAnnonceRepository : IDataRepository<NotificationModificationAnnonce, int>
{
}