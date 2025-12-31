using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services.Notifications.Events;

namespace API.Services.Notifications.Observers;

public class MessageNotificationObserver : INotificationObserver
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationMessageRepository _notificationMessageRepository;
    private readonly ILogger<MessageNotificationObserver> _logger;

    public MessageNotificationObserver(
        INotificationRepository notificationRepository,
        INotificationMessageRepository notificationMessageRepository,
        ILogger<MessageNotificationObserver> logger)
    {
        _notificationRepository = notificationRepository;
        _notificationMessageRepository = notificationMessageRepository;
        _logger = logger;
    }

    public NotificationTypeEnum[] GetSupportedTypes() => new[] { NotificationTypeEnum.NouveauMessage };

    public async Task HandleNotificationAsync(INotificationEvent notificationEvent)
    {
        if (notificationEvent is not NewMessageEvent messageEvent)
        {
            _logger.LogWarning("Event type mismatch in MessageNotificationObserver");
            return;
        }

        try
        {
            var notification = new Notification
            {
                UtilisateurId = messageEvent.TargetUserId,
                NotificationTypeId = (int)NotificationTypeEnum.NouveauMessage
            };

            await _notificationRepository.AddAsync(notification);

            var notificationMessage = new NotificationMessage
            {
                NotificationId = notification.NotificationId,
                MessageId = messageEvent.MessageId,
                MessagePreview = messageEvent.MessagePreview,
            };

            await _notificationMessageRepository.AddAsync(notificationMessage);

            _logger.LogInformation(
                "Notification créée pour l'utilisateur {UserId} concernant le message {MessageId}",
                messageEvent.TargetUserId, messageEvent.MessageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création de la notification message");
            throw;
        }
    }
}