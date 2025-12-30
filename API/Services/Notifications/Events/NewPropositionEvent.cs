namespace API.Services.Notifications.Events;

public class NewPropositionEvent : INotificationEvent
{
    public int TargetUserId { get; set; }
    public int SenderId { get; set; }
    public int DemandeId { get; set; }
    public int GetTargetUserId() => TargetUserId;
    public NotificationTypeEnum GetNotificationType() => NotificationTypeEnum.NouvelleProposition;
}