namespace API.Services.Interfaces;

public interface IDailyDeleteReadNotificationService
{
    Task DeleteReadOldNotifications();
}