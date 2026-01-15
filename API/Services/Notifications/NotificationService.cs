using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using API.Services.Interfaces;
using API.Services.VerificationSrvceV2;
using AutoMapper;
using Shared.DTO;
using Shared.DTO.Notification;
using Shared.Enums;

namespace API.Services;

public class  NotificationService : INotificationService
{
    private readonly IMapper _mapper;
    private readonly INotificationRepository _notificationManager;
    private readonly IFavorisRepository _favorisManager;
    private readonly IUtilisateurRepository _utilisateurRepository;
    private readonly IDataRepository<NotificationAvertissement, int> _notificationAvertissementManager;
    private readonly IDataRepository<NotificationCommercial, int> _notificationCommercialManager;
    private readonly IDataRepository<NotificationProposition, int> _notificationPropositionManager;
    private readonly IDataRepository<NotificationMessage, int> _notificationMessageManager;
    private readonly IDataRepository<NotificationNouvelleAnnonce, int> _notificationNouvelleAnnonceManager;
    private readonly IDataRepository<NotificationModificationAnnonce, int> _notificationModificationAnnonceManager;
    private readonly IDataRepository<NotificationAchatAnnonce, int> _notificationAchatAnnonceManager;
    private readonly INotificationMailService _mailService;
    private readonly IAnnonceRepository<Annonce, int, FilterDTO> _annonceRepository;
    private readonly IAbonnementRepository<Abonnement, int> _abonnementRepo;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationHubService _hubService;


    public NotificationService(
        IMapper mapper,
        INotificationRepository notificationManager,
        IFavorisRepository favorisManager,
        IUtilisateurRepository utilisateurRepository,
        IDataRepository<NotificationAvertissement, int> notificationAvertissementManager,
        IDataRepository<NotificationCommercial, int> notificationCommercialManager,
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
        _favorisManager = favorisManager;
        _notificationAvertissementManager = notificationAvertissementManager;
        _notificationCommercialManager = notificationCommercialManager; 
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
        _utilisateurRepository = utilisateurRepository;
    }

    public async Task CreateNotification(NotificationCreateDTO notificationDTO)
    {
        int notificationId;
        switch (notificationDTO)
        {
                
            case NotificationCommercialCreateDTO notificationCommercialCreateDTO:
                await CreateNotificationCommercial(notificationCommercialCreateDTO);
                break;
            case NotificationNouvelleAnnonceCreateDTO notificationNouvelleAnnonceCreateDTO:
                await CreateNouvelleAnnonceNotification(notificationNouvelleAnnonceCreateDTO);
                break;
            case NotificationAchatCreateDTO achatCreateDTO:
                await CreateNotificationAchat(achatCreateDTO);
                break;
            case NotificationMessageCreateDTO notificationMessageCreateDTO:
                notificationId = await GetBaseNotification(notificationMessageCreateDTO, notificationMessageCreateDTO.UtilisateurId);
                NotificationMessage notificationMessage = _mapper.Map<NotificationMessage>(notificationMessageCreateDTO,
                    opt =>
                    {
                        opt.Items["notificationId"] = notificationId;
                    });
                await _notificationMessageManager.AddAsync(notificationMessage);
                await UpdateNotificationCount(notificationMessageCreateDTO.UtilisateurId);
                break;
            case NotificationPropositionCreateDTO notificationPropositionCreateDTO:
                notificationId = await GetBaseNotification(notificationPropositionCreateDTO, notificationPropositionCreateDTO.UtilisateurId);
                NotificationProposition notificationProposition = _mapper.Map<NotificationProposition>(notificationPropositionCreateDTO,
                    opt =>
                    {
                        opt.Items["notificationId"] = notificationId;
                    });
                await _notificationPropositionManager.AddAsync(notificationProposition);
                await UpdateNotificationCount(notificationPropositionCreateDTO.UtilisateurId);
                break;
            case NotificationAvertissementCreateDTO notificationAvertissementCreateDTO:
                notificationId = await GetBaseNotification(notificationAvertissementCreateDTO, notificationAvertissementCreateDTO.UtilisateurId);
                NotificationAvertissement notificationAvertissement =  _mapper.Map<NotificationAvertissement>(notificationAvertissementCreateDTO,
                    opt =>
                    {
                        opt.Items["notificationId"] = notificationId;
                    });
                await _notificationAvertissementManager.AddAsync(notificationAvertissement);
                await UpdateNotificationCount(notificationAvertissementCreateDTO.UtilisateurId);
                break;
            
            case NotificationModificationAnnonceCreateDTO notificationModificationAnnonceCreateDTO:
                await CreateModificationAnnonceNotification(notificationModificationAnnonceCreateDTO);
                break;
        }
        
    }
    private async Task CreateModificationAnnonceNotification(NotificationModificationAnnonceCreateDTO notificationModificationAnnonceCreateDTO)
    {
        IEnumerable<Utilisateur> utilisateurs = await _favorisManager.GetUtilisateurByAnnonceId(notificationModificationAnnonceCreateDTO.AnnonceId);
        foreach (var user in utilisateurs)
        {
            if (user.PreferenceNotifMail && user.ValidEmail)
            {
                await _mailService.NotifyAnnonceUpdatedAsync(notificationModificationAnnonceCreateDTO.AnnonceTitle, notificationModificationAnnonceCreateDTO.AnnonceId, user.Email);
            }
            int notificationId = await GetBaseNotification(notificationModificationAnnonceCreateDTO, user.UtilisateurId);
            NotificationModificationAnnonce notificationModificationAnnonce =
                _mapper.Map<NotificationModificationAnnonce>(notificationModificationAnnonceCreateDTO,
                    opt =>
                    {
                        opt.Items["notificationId"] = notificationId;
                    });
            await _notificationModificationAnnonceManager.AddAsync(notificationModificationAnnonce);
            await UpdateNotificationCount(user.UtilisateurId);
        }
    }
    private async Task CreateNouvelleAnnonceNotification(NotificationNouvelleAnnonceCreateDTO notificationNouvelleAnnonceDTO)
    {
        var followers = await _abonnementRepo.GetAllFollowersByUtilisateurSuivi(notificationNouvelleAnnonceDTO.UtilisateurIdFollowed);
        foreach (var user in followers)
        {
            if (user.UtilisateurSuiveur.PreferenceNotifMail)
            {
                await _mailService.NotifyNewAnnonceAsync(notificationNouvelleAnnonceDTO.AnnonceTitle, notificationNouvelleAnnonceDTO.UtilisateurLogin, notificationNouvelleAnnonceDTO.AnnonceId, user.UtilisateurSuiveur.Email);
            }
            int notificationId = await GetBaseNotification(notificationNouvelleAnnonceDTO, user.UtilisateurSuiveurId);
            NotificationNouvelleAnnonce notificationNouvelleAnnonce =
                _mapper.Map<NotificationNouvelleAnnonce>(notificationNouvelleAnnonceDTO,
                    opt =>
                    {
                        opt.Items["notificationId"] = notificationId;
                    });
            await _notificationNouvelleAnnonceManager.AddAsync(notificationNouvelleAnnonce);
            await UpdateNotificationCount(user.UtilisateurSuiveurId);
        }
    }
    private async Task CreateNotificationAchat(NotificationAchatCreateDTO notificationAchatDTO)
    {
        
        IEnumerable<Utilisateur> utilisateurs = await _favorisManager.GetUtilisateurByAnnonceId(notificationAchatDTO.AnnonceId);
        int acheteurId = await _currentUserService.GetUserIdOrThrow();
        foreach (var user in utilisateurs)
        {
            if (user.UtilisateurId != acheteurId)
            {
                int notificationId = await GetBaseNotification(notificationAchatDTO, user.UtilisateurId);
                NotificationAchatAnnonce notificationAchatAnnonce = _mapper.Map<NotificationAchatAnnonce>(notificationAchatDTO,opt =>
                {
                    opt.Items["notificationId"] = notificationId;
                });
                await _notificationAchatAnnonceManager.AddAsync(notificationAchatAnnonce);
                await UpdateNotificationCount(user.UtilisateurId);
            }
        }
    }
   
    private async Task<int> GetBaseNotification(NotificationCreateDTO notificationDTO, int userId)
    {
        Notification notification = _mapper.Map<Notification>(notificationDTO,
            opt =>
            {
                opt.Items["utilisateurId"] = userId;
            });
        await _notificationManager.AddAsync(notification);
        return notification.NotificationId;
    }
    private async Task CreateNotificationCommercial(NotificationCommercialCreateDTO notificationCommercialCreate)
    {
        var utilisateurs = await _utilisateurRepository.GetAllAsync();
        foreach (var user in utilisateurs)
        {
            int notificationId = await GetBaseNotification(notificationCommercialCreate, user.UtilisateurId);
            NotificationCommercial notificationCommercial = _mapper.Map<NotificationCommercial>(notificationCommercialCreate, opt => opt.Items["notificationId"] = notificationId);
            
            await _notificationCommercialManager.AddAsync(notificationCommercial);
            await UpdateNotificationCount(user.UtilisateurId);
        }
    }
    private async Task UpdateNotificationCount(int userId)
    {
        int newCount = await _notificationManager.GetNotificationsUnreadCountByUserId(userId);
        await _hubService.UpdateNotificationCount(userId, newCount);
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
    public async Task DeleteMessagesNotificationByConversationId(int conversationId, int userId)
    {
        await _notificationManager.DeleteMessageNotificationByConversationId(conversationId, userId);
        int newCount = await _notificationManager.GetNotificationsUnreadCountByUserId(userId);
        await _hubService.UpdateNotificationCount(userId, newCount);
    }
}