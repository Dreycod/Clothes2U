namespace API.Services.Notifications.Events;

public class ModificationAnnonceEvent : INotificationEvent
{
    public int AnnonceId { get; set; }
    public int CreatorId { get; set; }

    public int GetTargetUserId() => throw new NotImplementedException("Use InterestedUserIds instead");
    public NotificationTypeEnum GetNotificationType() => NotificationTypeEnum.ModificationAnnonce;
}