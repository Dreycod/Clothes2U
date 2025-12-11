using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services.Notifications.Events;

namespace API.Services.Notifications.Observers;

public class ModificationAnnonceNotificationObserver : INotificationObserver
{
    private readonly INotificationRepository _notificationRepository;
    private readonly INotificationModificationAnnonceRepository _notificationModificationAnnonceRepository;
    private readonly IAbonnementRepository<Abonnement, int>  _abonnementManager;
    private readonly ILogger<ModificationAnnonceNotificationObserver> _logger;

    public ModificationAnnonceNotificationObserver(
        INotificationRepository notificationRepository,
        INotificationModificationAnnonceRepository notificationModificationAnnonceRepository,
        IAbonnementRepository<Abonnement, int>  abonnementRepository,
        ILogger<ModificationAnnonceNotificationObserver> logger)
    {
        _notificationRepository = notificationRepository;
        _notificationModificationAnnonceRepository = notificationModificationAnnonceRepository;
        _abonnementManager = abonnementRepository;
        _logger = logger;
    }

    public NotificationTypeEnum[] GetSupportedTypes() => new[] { NotificationTypeEnum.ModificationAnnonce };

    public async Task HandleNotificationAsync(INotificationEvent notificationEvent)
    {
        if (notificationEvent is not ModificationAnnonceEvent modificationEvent)
        {
            _logger.LogWarning("Event type mismatch in ModificationAnnonceNotificationObserver");
            return;
        }

        try
        {
            IEnumerable<Abonnement> followers = await _abonnementManager.GetAllFollowersByUtilisateurSuivi(modificationEvent.CreatorId);
            foreach (Abonnement follower in followers)
            {
                if (follower.UtilisateurSuiveurId == modificationEvent.CreatorId)
                    continue;

                var notification = new Notification
                {
                    UtilisateurId = follower.UtilisateurSuiveurId,
                    NotificationTypeId = (int)NotificationTypeEnum.ModificationAnnonce
                };

                await _notificationRepository.AddAsync(notification);

                var notificationModification = new NotificationModificationAnnonce
                {
                    NotificationId = notification.NotificationId,
                    AnnonceId = modificationEvent.AnnonceId
                };

                await _notificationModificationAnnonceRepository.AddAsync(notificationModification);

                _logger.LogInformation(
                    "Notification créée pour l'utilisateur {UserId} concernant la modification de l'annonce {AnnonceId}",
                    follower.UtilisateurSuiveurId, modificationEvent.AnnonceId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création des notifications modification annonce");
            throw;
        }
    }
}