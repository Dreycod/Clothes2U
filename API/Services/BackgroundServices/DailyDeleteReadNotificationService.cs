using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services.Interfaces;

namespace API.Services.BackgroundServices;

public class DailyDeleteReadNotificationService : IDailyDeleteReadNotificationService
{
    private readonly INotificationRepository _notificationManager;

    public DailyDeleteReadNotificationService(INotificationRepository notificationManager)
    {
        _notificationManager = notificationManager;
    }

    public async Task DeleteReadOldNotifications()
    {
        IEnumerable<Notification> notifications = await _notificationManager.GetReadNotifications();
        DateTime twoWeeksAgo = DateTime.UtcNow.AddDays(-14);
        foreach (var notification in notifications)
        {
            if (notification.DateCreation < twoWeeksAgo)
            {
                await _notificationManager.DeleteAsync(notification);
            }
        }
    }
}