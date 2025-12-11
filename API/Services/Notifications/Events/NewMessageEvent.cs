namespace API.Services.Notifications.Events;

public class NewMessageEvent : INotificationEvent
{
    public int TargetUserId { get; set; }
    public int MessageId { get; set; }
    public int SenderId { get; set; }
    public string MessagePreview { get; set; }

    public int GetTargetUserId() => TargetUserId;
    public NotificationTypeEnum GetNotificationType() => NotificationTypeEnum.NouveauMessage;
}