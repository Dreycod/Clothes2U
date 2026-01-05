using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Shared.DTO.Notification;

namespace API.Services;

public class NotificationService : INotificationService
{
    private readonly IMapper _mapper;
    private readonly INotificationRepository _notificationManager;
    private readonly INotificationAvertissementRepository _notificationAvertissementManager;
    private readonly INotificationPropositionRepository _notificationPropositionManager;
    private readonly INotificationMessageRepository _notificationMessageManager;
    private readonly INotificationNouvelleAnnonceRepository _notificationNouvelleAnnonceManager;
    private readonly INotificationModificationAnnonceRepository _notificationModificationAnnonceManager;

    public NotificationService(
        IMapper mapper,
        INotificationRepository notificationManager,
        INotificationAvertissementRepository notificationAvertissementManager,
        INotificationPropositionRepository notificationPropositionManager,
        INotificationMessageRepository notificationMessageManager,
        INotificationNouvelleAnnonceRepository notificationNouvelleAnnonceManager,
        INotificationModificationAnnonceRepository notificationModificationAnnonceManager
    )
    {
        _mapper = mapper;
        _notificationManager = notificationManager;
        _notificationAvertissementManager = notificationAvertissementManager;
        _notificationPropositionManager = notificationPropositionManager;
        _notificationMessageManager = notificationMessageManager;
        _notificationNouvelleAnnonceManager = notificationNouvelleAnnonceManager;
        _notificationModificationAnnonceManager = notificationModificationAnnonceManager;
    }

    public async Task CreateNotification(NotificationCreateDTO notificationDTO)
    {
        Console.WriteLine("----------------------------------------------------------------------------------------------------");
        Notification notification = _mapper.Map<Notification>(notificationDTO);
        await _notificationManager.AddAsync(notification);
        notificationDTO.NotificationId = notification.NotificationId;
        switch (notificationDTO)
        {
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
    }
}