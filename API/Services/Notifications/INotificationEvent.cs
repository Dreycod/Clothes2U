namespace API.Services.Notifications;

public interface INotificationEvent
{
    int GetTargetUserId();
    NotificationTypeEnum GetNotificationType();
}

public enum NotificationTypeEnum
{
    NouveauMessage = 1,
    ModificationAnnonce = 4, 
    NouvelleAnnonce = 5,
    NouvelleProposition = 6,
}