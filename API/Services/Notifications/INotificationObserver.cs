namespace API.Services.Notifications;

public interface INotificationObserver
{
    Task HandleNotificationAsync(INotificationEvent notificationEvent);
    NotificationTypeEnum[] GetSupportedTypes();
}