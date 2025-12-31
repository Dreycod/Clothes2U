using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services.Notifications.Events;

namespace API.Services.Notifications.Observers;

public class PropositionNotificationObserver : INotificationObserver
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationPropositionRepository _notificationPropositionManager;
    private readonly ILogger<PropositionNotificationObserver> _logger;
    
    public PropositionNotificationObserver(
        INotificationRepository notificationRepository,
        INotificationPropositionRepository notificationMessageRepository,
        ILogger<PropositionNotificationObserver> logger)
    {
        _notificationRepository = notificationRepository;
        _notificationPropositionManager = notificationMessageRepository;
        _logger = logger;
    }
    
    public NotificationTypeEnum[] GetSupportedTypes() => new[] { NotificationTypeEnum.NouvelleProposition };

    public async Task HandleNotificationAsync(INotificationEvent notificationEvent)
    {
        if (notificationEvent is not NewPropositionEvent propositionEvent)
        {
            _logger.LogWarning("Event type mismatch in MessageNotificationObserver");
            return;
        }

        try
        {
            var notification = new Notification
            {
                UtilisateurId = propositionEvent.TargetUserId,
                NotificationTypeId = (int)NotificationTypeEnum.NouveauMessage
            };

            await _notificationRepository.AddAsync(notification);

            var notificationProposition = new NotificationProposition()
            {
                NotificationId = notification.NotificationId,
                PropositionId = propositionEvent.DemandeId
            };

            await _notificationPropositionManager.AddAsync(notificationProposition);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création de la notification message");
            throw;
        }
    }
}