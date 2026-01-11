using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services.Interfaces;
using API.Services.VerificationSrvceV2;
using AutoMapper;
using Shared.DTO;
using Shared.DTO.Notification;

namespace API.Services;

public class  NotificationService : INotificationService
{
    private readonly IMapper _mapper;
    private readonly INotificationRepository _notificationManager;
    private readonly IDataRepository<NotificationAvertissement, int> _notificationAvertissementManager;
    private readonly IDataRepository<NotificationProposition, int> _notificationPropositionManager;
    private readonly IDataRepository<NotificationMessage, int> _notificationMessageManager;
    private readonly IDataRepository<NotificationNouvelleAnnonce, int> _notificationNouvelleAnnonceManager;
    private readonly IDataRepository<NotificationModificationAnnonce, int> _notificationModificationAnnonceManager;
    private readonly IDataRepository<NotificationAchatAnnonce, int> _notificationAchatAnnonceManager;
    private readonly INotificationMailService _mailService;
    private readonly IAnnonceRepository<Annonce, int, FilterDTO> _annonceRepository;
    private readonly IAbonnementRepository<Abonnement, int> _abonnementRepo;
    private readonly IAbonnementRepository<Abonnement, int> abonnementRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationHubService _hubService;
    

    public NotificationService(
        IMapper mapper,
        INotificationRepository notificationManager,
        IDataRepository<NotificationAvertissement, int> notificationAvertissementManager,
        IDataRepository<NotificationProposition, int> notificationPropositionManager,
        IDataRepository<NotificationMessage, int> notificationMessageManager,
        IDataRepository<NotificationNouvelleAnnonce, int>  notificationNouvelleAnnonceManager,
        IDataRepository<NotificationModificationAnnonce, int> notificationModificationAnnonceManager,
        IDataRepository<NotificationAchatAnnonce, int>  notificationAchatAnnonceManager,
        INotificationMailService mailService,
        IAnnonceRepository<Annonce, int, FilterDTO> annonceRepository,
        IAbonnementRepository<Abonnement, int> abonnementRepo,
        ICurrentUserService currentUserService,
        INotificationHubService hubService
    )
    {
        _mapper = mapper;
        _notificationManager = notificationManager;
        _notificationAvertissementManager = notificationAvertissementManager;
        _notificationPropositionManager = notificationPropositionManager;
        _notificationMessageManager = notificationMessageManager;
        _notificationNouvelleAnnonceManager = notificationNouvelleAnnonceManager;
        _notificationModificationAnnonceManager = notificationModificationAnnonceManager;
        _notificationAchatAnnonceManager = notificationAchatAnnonceManager;
        _mailService = mailService;
        _hubService = hubService;
        _annonceRepository = annonceRepository;
        _abonnementRepo = abonnementRepo;
        _currentUserService = currentUserService;
    }

    public async Task CreateNotification(NotificationCreateDTO notificationDTO)
    {
        Notification notification = _mapper.Map<Notification>(notificationDTO);
        await _notificationManager.AddAsync(notification);
        notificationDTO.NotificationId = notification.NotificationId;
        switch (notificationDTO)
        {
            case NotificationAchatCreateDTO achatCreateDTO:
                NotificationAchatAnnonce notificationAchatAnnonce = _mapper.Map<NotificationAchatAnnonce>(achatCreateDTO);
                await _notificationAchatAnnonceManager.AddAsync(notificationAchatAnnonce);
                break;
            case NotificationMessageCreateDTO notificationMessageCreateDTO:
                NotificationMessage notificationMessage = _mapper.Map<NotificationMessage>(notificationMessageCreateDTO);
                await _notificationMessageManager.AddAsync(notificationMessage);
                break;
            case NotificationPropositionCreateDTO notificationPropositionCreateDTO:
                NotificationProposition notificationProposition = _mapper.Map<NotificationProposition>(notificationPropositionCreateDTO);
                await _notificationPropositionManager.AddAsync(notificationProposition);
                break;
            case NotificationAvertissementCreateDTO notificationAvertissementCreateDTO:
                NotificationAvertissement notificationAvertissement =  _mapper.Map<NotificationAvertissement>(notificationAvertissementCreateDTO);
                await _notificationAvertissementManager.AddAsync(notificationAvertissement);
                break;
            case NotificationNouvelleAnnonceCreateDTO notificationNouvelleAnnonceCreateDTO:
                NotificationNouvelleAnnonce notificationNouvelleAnnonce =
                _mapper.Map<NotificationNouvelleAnnonce>(notificationNouvelleAnnonceCreateDTO);
                await _notificationNouvelleAnnonceManager.AddAsync(notificationNouvelleAnnonce);
                break;
            case NotificationModificationAnnonceCreateDTO notificationModificationAnnonceCreateDTO:
                NotificationModificationAnnonce notificationModificationAnnonce = _mapper.Map<NotificationModificationAnnonce>(notificationModificationAnnonceCreateDTO);
                await _notificationModificationAnnonceManager.AddAsync(notificationModificationAnnonce);
                break;
        }
        int newCount = await _notificationManager.GetNotificationsUnreadCountByUserId(notificationDTO.UtilisateurId);
        await _hubService.UpdateNotificationCount(notificationDTO.UtilisateurId, newCount);
    }

    public async Task CreateModificationAnnonceNotification(int annonceId)
    {
        Annonce annonce = await _annonceRepository.GetByIdAsync(annonceId);
        var users = annonce.UtilisateursFavoris
            .Select(f => f.Utilisateur);
        foreach (var user in users)
        {
            if (user.PreferenceNotifMail)
            {
                await _mailService.NotifyAnnonceUpdatedAsync(annonce, user.Email);
            }

            NotificationModificationAnnonceCreateDTO notif = new NotificationModificationAnnonceCreateDTO
            {
                UtilisateurId = user.UtilisateurId,
                AnnonceId = annonceId,
                TypeId = 4
            };
            await CreateNotification(notif);
        }
    }
    public async Task CreateNouvelleAnnonceNotification(int annonceId)
    {
        Annonce annonce = await _annonceRepository.GetByIdAsync(annonceId);
        var followers = await _abonnementRepo.GetAllFollowersByUtilisateurSuivi(annonce.UtilisateurId);
        foreach (var user in followers)
        {
            if (user.UtilisateurSuiveur.PreferenceNotifMail)
            {
                await _mailService.NotifyNewAnnonceAsync(annonce, user.UtilisateurSuiveur.Email);
            }

            NotificationNouvelleAnnonceCreateDTO notif = new NotificationNouvelleAnnonceCreateDTO
            {
                UtilisateurId = user.UtilisateurSuiveur.UtilisateurId,
                AnnonceId = annonceId,
                TypeId = 4
            };
            await CreateNotification(notif);
        }
    }

    public async Task DeleteAnnonceNotificationForUser(int annonceId)
    {
        int? userId = await _currentUserService.GetUserId();
        if (userId != null)
        {
            await _notificationManager.DeleteNotificationAnnonceForUser(annonceId, (int)userId);
            int newCount = await _notificationManager.GetNotificationsUnreadCountByUserId(userId.Value);
            await _hubService.UpdateNotificationCount(userId.Value, newCount);
        }
        

    }

    public async Task CreateNotificationAchat(int annonceId)
    {
        Annonce annonce = await _annonceRepository.GetByIdAsync(annonceId);
        var users = annonce.UtilisateursFavoris
            .Select(f => f.Utilisateur);
        int acheteurId = await _currentUserService.GetUserIdOrThrow();
        foreach (var user in users)
        {
            if (user.UtilisateurId != acheteurId)
            {
                NotificationAchatCreateDTO notif = new NotificationAchatCreateDTO
                {
                    UtilisateurId = user.UtilisateurId,
                    AnnonceId = annonceId,
                    TypeId = 6
                };
                
                await CreateNotification(notif);
            }
        }
    }

    public async Task DeleteMessagesNotificationByConversationId(int conversationId, int userId)
    {
        await _notificationManager.DeleteMessageNotificationByConversationId(conversationId, userId);
        int newCount = await _notificationManager.GetNotificationsUnreadCountByUserId(userId);
        await _hubService.UpdateNotificationCount(userId, newCount);
    }
}