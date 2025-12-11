using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services.Notifications.Events;

namespace API.Services.Notifications.Observers;

public class NouvelleAnnonceNotificationObserver : INotificationObserver
{
    private readonly INotificationRepository _notificationRepository;
    private readonly IAbonnementRepository<Abonnement, int> _abonnementManager;
    private readonly INotificationNouvelleAnnonceRepository _notificationNouvelleAnnonceRepository;
    
    private readonly ILogger<NouvelleAnnonceNotificationObserver> _logger;

    public NouvelleAnnonceNotificationObserver(
        INotificationRepository notificationRepository,
        IAbonnementRepository<Abonnement, int> abonnementRepository,
        INotificationNouvelleAnnonceRepository notificationNouvelleAnnonceRepository,
        ILogger<NouvelleAnnonceNotificationObserver> logger)
    {
        _notificationRepository = notificationRepository;
        _notificationNouvelleAnnonceRepository = notificationNouvelleAnnonceRepository;
        _abonnementManager = abonnementRepository;
        _logger = logger;
    }

    public NotificationTypeEnum[] GetSupportedTypes() => new[] { NotificationTypeEnum.NouvelleAnnonce };

    public async Task HandleNotificationAsync(INotificationEvent notificationEvent)
    {
        if (notificationEvent is not NewAnnonceEvent annonceEvent)
        {
            _logger.LogWarning("Event type mismatch in NouvelleAnnonceNotificationObserver");
            return;
        }

        try
        {
            IEnumerable<Abonnement> followers = await _abonnementManager.GetAllFollowersByUtilisateurSuivi(annonceEvent.CreatorId);
            foreach (Abonnement follower in followers)
            {
                var notification = new Notification
                {
                    UtilisateurId = follower.UtilisateurSuiveurId,
                    NotificationTypeId = (int)NotificationTypeEnum.NouvelleAnnonce
                };

                await _notificationRepository.AddAsync(notification);

                var notificationNouvelleAnnonce = new NotificationNouvelleAnnonce
                {
                    NotificationId = notification.NotificationId,
                    AnnonceId = annonceEvent.AnnonceId
                };

                await _notificationNouvelleAnnonceRepository.AddAsync(notificationNouvelleAnnonce);

                _logger.LogInformation(
                    "Notification créée pour l'utilisateur {UserId} concernant la nouvelle annonce {AnnonceId}",
                    follower.UtilisateurSuiveurId, annonceEvent.AnnonceId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création des notifications nouvelle annonce");
            throw;
        }
    }
}