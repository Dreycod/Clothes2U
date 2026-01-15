using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using API.Services.Interfaces;
using API.Services.VerificationSrvceV2;
using AutoMapper;
using Shared.DTO;
using Shared.DTO.Decision;
using Shared.DTO.Notification;
using Shared.Enums;


namespace API.Services;

public class DecisionService : IDecisionService
{
    private readonly IDecisionRepository _decisionManager;
    private readonly IUtilisateurRepository _utilisateurManager;
    private readonly IAnnonceRepository<Annonce, int, FilterDTO> _annonceManager;
    private readonly ISignalementRepository _signalementManager;
    private readonly INoteUtilisateurRepository _noteUtilisateurManager;
    private readonly IConversationRepository<Conversation, int> _conversationManager;
    private readonly IMapper _mapper;
    private readonly INotificationMailService _mailService;
    private readonly INotificationService _notificationService;

    public DecisionService(
        IDecisionRepository decisionRepository,
        IUtilisateurRepository utilisateurRepository,
        ISignalementRepository signalementRepository,
        IAnnonceRepository<Annonce, int, FilterDTO> annonceRepository,
        INoteUtilisateurRepository noteUtilisateurRepository,
        IConversationRepository<Conversation, int> conversationRepository,
        INotificationMailService mailService,
        INotificationService notificationService,
        IMapper mapper)
    {
        _decisionManager = decisionRepository;
        _utilisateurManager = utilisateurRepository;
        _signalementManager = signalementRepository;
        _annonceManager = annonceRepository;
        _noteUtilisateurManager = noteUtilisateurRepository;
        _conversationManager = conversationRepository;
        _mailService = mailService;
        _notificationService = notificationService;
        _mapper = mapper;
    }
    public async Task<IEnumerable<DecisionDTO>> GetAllDecisionsByModeratorAsync(int moderatorId)
    {
        var decisions = await _decisionManager.GetAllDecisionsByModerateurId(moderatorId);
        return _mapper.Map<IEnumerable<DecisionDTO>>(decisions);
    }

    public async Task<DecisionDetailDTO?> GetDecisionByIdAsync(int decisionId)
    {
        var decision = await _decisionManager.GetByIdAsync(decisionId);
        if (decision == null)
        {
            return null;
        }
        return _mapper.Map<DecisionDetailDTO>(decision);
    }
    public async Task<Decision> CreateDecisionAsync(DecisionPostDTO decisionDTO, int moderatorId)
    {
        var signalement = await _signalementManager.GetByIdAsync(decisionDTO.SignalementId);
        if (signalement == null)
        {
            throw new KeyNotFoundException("Signalement introuvable");
        }
        var utilisateurSanctionne = await _utilisateurManager.GetByIdAsync(decisionDTO.UtilisateurId);
        if (utilisateurSanctionne == null)
        {
            throw new KeyNotFoundException("Utilisateur introuvable");
        }
        var elementDecision = await CreateElementDecisionAsync(decisionDTO.ElementDecision);
        var decision = new Decision
        {
            ElementDecision = elementDecision,
            ModerateurId = moderatorId,
            UtilisateurId = decisionDTO.UtilisateurId,
            DecisionDate = DateTime.UtcNow
        };
        await ApplySanctionAsync(decision, decisionDTO, utilisateurSanctionne);
        var createdDecision = await _decisionManager.AddAsync(decision);
        await CleanupSignalementsAsync(decisionDTO.UtilisateurId, signalement);

        return createdDecision;
    }
    private async Task<ElementDecision> CreateElementDecisionAsync(ElementDecisionDTO dto)
    {
        var elementDecision = new ElementDecision();

        switch (dto)
        {
            case ElementDecisionAnnonceDTO annonce:
                await _annonceManager.SuspendElement(annonce.AnnonceId);
                elementDecision.ElementDecisionAnnonce = new ElementDecisionAnnonce
                {
                    AnnonceId = annonce.AnnonceId
                };
                break;

            case ElementDecisionMessageDTO message:
                await _conversationManager.SuspendElement(message.MessageId);
                elementDecision.ElementDecisionMessage = new ElementDecisionMessage
                {
                    MessageId = message.MessageId
                };
                break;

            case ElementAvisDTO avis:
                await _noteUtilisateurManager.SuspendElement(avis.AvisId);
                elementDecision.ElementDecisionAvis = new ElementDecisionAvis
                {
                    AvisId = avis.AvisId
                };
                break;

            case ElementUtilisateurDTO:
                elementDecision.ElementDecisionUtilisateur = new ElementDecisionUtilisateur();
                break;

            default:
                throw new ArgumentException("Type d'élément de décision non reconnu");
        }

        return elementDecision;
    }
    private async Task ApplySanctionAsync(Decision decision, DecisionPostDTO decisionDTO, Utilisateur utilisateur)
    {
        switch (decisionDTO)
        {
            case DecisionAvertissementPostDTO avertissement:
                await ApplyAvertissementAsync(decision, avertissement);
                break;

            case SanctionSuspensionPostDTO suspension:
                await ApplySuspensionAsync(decision, suspension, utilisateur);
                break;

            case SanctionBannissementPostDTO bannissement:
                await ApplyBannissementAsync(decision, utilisateur);
                break;

            default:
                throw new ArgumentException("Type de décision non reconnu");
        }
    }
    private async Task ApplyAvertissementAsync(Decision decision, DecisionAvertissementPostDTO avertissement)
    {
        decision.DecisionAvertissement = new DecisionAvertissement();

        var notificationAvertissement = new NotificationAvertissementCreateDTO
        {
            TypeId = (int)TypeNotification.Avertissement,
            MessageModerateur = avertissement.MessageModerateur,
            UtilisateurId = avertissement.UtilisateurId,
        };

        await _notificationService.CreateNotification(notificationAvertissement);
    }
    private async Task ApplySuspensionAsync(Decision decision, SanctionSuspensionPostDTO suspension, Utilisateur utilisateur)
    {
        decision.DecisionSanction = new DecisionSanction
        {
            EstEnCours = true,
            SanctionSuspension = new SanctionSuspension
            {
                DateFinSuspension = suspension.DateFinSuspension
            }
        };

        await _utilisateurManager.SuspendUser(suspension.UtilisateurId);
        await _mailService.NotifyUserStatusChangedAsync(utilisateur, utilisateur.StatutId);
    }
    private async Task ApplyBannissementAsync(Decision decision, Utilisateur utilisateur)
    {
        decision.DecisionSanction = new DecisionSanction
        {
            EstEnCours = true,
            SanctionBannissement = new SanctionBannissement()
        };

        await _utilisateurManager.BanUser(utilisateur.UtilisateurId);
        await _mailService.NotifyUserStatusChangedAsync(utilisateur, utilisateur.StatutId);
    }
    private async Task CleanupSignalementsAsync(int utilisateurId, Signalement signalement)
    {
        await _signalementManager.DeleteSignalementByUserId(utilisateurId);
        await _signalementManager.DeleteAsync(signalement);
    }
}