namespace API.Services.Notifications.Events;

public class NewAnnonceEvent : INotificationEvent
{
    public int AnnonceId { get; set; }
    public int CreatorId { get; set; }

    public int GetTargetUserId() => throw new NotImplementedException("Use FollowerIds instead");
    public NotificationTypeEnum GetNotificationType() => NotificationTypeEnum.NouvelleAnnonce;
}