using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using API.Services;
using API.Services.VerificationSrvceV2;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using Shared.DTO.Decision;
using Shared.DTO.Notification;
using Shared.Enums;

namespace API.Controllers;




[ApiController]
[Route("api/[controller]")]
public class DecisionController : ControllerBase
{
    private readonly IDecisionRepository _decisionManager;
    private readonly IUtilisateurRepository _utilisateurRepository;
    private readonly IAnnonceRepository<Annonce, int, FilterDTO> _annonceManager;
    private readonly ISignalementRepository _signalementManager;
    private readonly INoteUtilisateurRepository _noteUtilisateurManager;
    private readonly IConversationRepository<Conversation, int> _conversationManager;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly INotificationMailService  _mailService;
    private readonly INotificationService _notificationService;

    public DecisionController(
        IDecisionRepository decisionManager,
        IUtilisateurRepository utilisateurRepository,
        ISignalementRepository signalementManager,
        ICurrentUserService currentUserService,
        IAnnonceRepository<Annonce, int, FilterDTO> annonceManager,
        INoteUtilisateurRepository noteUtilisateurManager,
        IConversationRepository<Conversation, int> conversationManager,
        INotificationMailService mailService,
        INotificationService notificationService,
        IMapper mapper)
    {
        _currentUserService =  currentUserService;
        _signalementManager = signalementManager;
        _utilisateurRepository = utilisateurRepository;
        _decisionManager = decisionManager;
        _annonceManager = annonceManager;
        _noteUtilisateurManager = noteUtilisateurManager;
        _conversationManager = conversationManager;
        _mapper = mapper;
        _mailService = mailService;
        _notificationService = notificationService;
    }
    /// <summary>
    /// Récupère toutes les décisions prises par le modérateur connecté.
    /// </summary>
    /// <returns>La liste des décisions du modérateur.</returns>
    /// <response code="200">Retourne la liste des décisions.</response>
    /// <response code="401">L'utilisateur n'est pas authentifié.</response>
    /// <response code="403">L'utilisateur n'a pas les droits nécessaires (Admin ou Modérateur requis).</response>
    /// <response code="500">Erreur interne du serveur.</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Moderateur")]
    [ProducesResponseType(typeof(IEnumerable<DecisionDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ActionResult<DecisionDTO>>> GetAllDecisionsByModerateurId()
    {
        int userId = await _currentUserService.GetUserIdOrThrow();
        IEnumerable<Decision> decisions = await _decisionManager.GetAllDecisionsByModerateurId((int)userId);
        IEnumerable<DecisionDTO> decisionsDTO = _mapper.Map<IEnumerable<DecisionDTO>>(decisions);
        return Ok(decisionsDTO);
    }
    /// <summary>
    /// Récupère les détails d'une décision spécifique par son identifiant.
    /// Le type de retour varie selon le type de décision (avertissement, suspension ou bannissement).
    /// </summary>
    /// <param name="id">L'identifiant de la décision.</param>
    /// <returns>Les détails de la décision (peut être un avertissement, une suspension ou un bannissement).</returns>
    /// <response code="200">Retourne les détails de la décision.</response>
    /// <response code="404">La décision n'existe pas.</response>
    /// <response code="401">L'utilisateur n'est pas authentifié.</response>
    /// <response code="403">L'utilisateur n'a pas les droits nécessaires (Admin ou Modérateur requis).</response>
    /// <response code="500">Erreur interne du serveur.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DecisionDetailDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Authorize(Roles = "Admin,Moderateur")]
    public async Task<ActionResult> GetDecisionById(int id)
    {
        Decision decision = await _decisionManager.GetByIdAsync(id);
        if (decision == null)
        {
            return NotFound();
        }
        DecisionDetailDTO decisionDTO = _mapper.Map<DecisionDetailDTO>(decision);
        return new ObjectResult(decisionDTO)
        {
            StatusCode = StatusCodes.Status200OK,
            DeclaredType = typeof(DecisionDetailDTO)
        };
    }
    /// <summary>
    /// Crée une nouvelle décision de modération suite à un signalement.
    /// Peut être un avertissement, une suspension ou un bannissement.
    /// La décision entraîne automatiquement les actions suivantes :
    /// - Avertissement : envoi d'une notification à l'utilisateur
    /// - Suspension : suspension temporaire du compte et envoi d'un email
    /// - Bannissement : bannissement définitif du compte et envoi d'un email
    /// Les éléments concernés (annonce, message, avis) sont également suspendus.
    /// Le signalement est supprimé après la création de la décision.
    /// </summary>
    /// <param name="decisionDTO">Les données de la décision à créer (type variant selon l'action : avertissement, suspension ou bannissement).</param>
    /// <returns>La décision créée avec son identifiant.</returns>
    /// <response code="201">La décision a été créée avec succès.</response>
    /// <response code="400">Les données sont invalides ou le type de décision n'est pas reconnu.</response>
    /// <response code="404">Le signalement ou l'utilisateur n'existe pas.</response>
    /// <response code="401">L'utilisateur n'est pas authentifié.</response>
    /// <response code="403">L'utilisateur n'a pas les droits nécessaires (Admin ou Modérateur requis).</response>
    /// <response code="500">Erreur interne du serveur.</response>
    [HttpPost]
    [Authorize(Roles = "Admin,Moderateur")]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DecisionPostDTO>> CreateDecision([FromBody] DecisionPostDTO decisionDTO)
    {
        try
        {
            Signalement signalement = await _signalementManager.GetByIdAsync(decisionDTO.SignalementId);
            
            if (signalement == null)
            {
                return NotFound("Signalement introuvable");
            }
            int userId = await _currentUserService.GetUserIdOrThrow();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            Utilisateur utilisateurSanctionne = await _utilisateurRepository.GetByIdAsync(decisionDTO.UtilisateurId);
            if (utilisateurSanctionne == null)
            {
                return NotFound("Utilisateur introuvable");
            }
            var elementDecision = await CreateElementDecision(decisionDTO.ElementDecision);
            var decision = new Decision
            {
                ElementDecision = elementDecision,
                ModerateurId = userId,
                UtilisateurId = decisionDTO.UtilisateurId,
                DecisionDate = DateTime.UtcNow
            };
            switch (decisionDTO)
            {
                case DecisionAvertissementPostDTO avertissement:
                    decision.DecisionAvertissement = new DecisionAvertissement();
                    NotificationAvertissementCreateDTO notificationAvertissement = new NotificationAvertissementCreateDTO()
                    {
                        TypeId = (int)TypeNotification.Avertissement,
                        MessageModerateur = avertissement.MessageModerateur,
                        UtilisateurId = avertissement.UtilisateurId,
                    };
                    await _notificationService.CreateNotification(notificationAvertissement);
                    break;

                case SanctionSuspensionPostDTO suspension:
                    decision.DecisionSanction = new DecisionSanction
                    {
                        EstEnCours = true,
                        SanctionSuspension = new SanctionSuspension
                        {
                            DateFinSuspension = suspension.DateFinSuspension
                        }
                    };
                    
                    await _utilisateurRepository.SuspendUser(decisionDTO.UtilisateurId);
                    await _mailService.NotifyUserStatusChangedAsync(
                        utilisateurSanctionne, 
                        utilisateurSanctionne.StatutId);
                    break;

                case SanctionBannissementPostDTO bannissement:
                    decision.DecisionSanction = new DecisionSanction
                    {
                        EstEnCours = true,
                        SanctionBannissement = new SanctionBannissement()
                    };
                    
                    await _utilisateurRepository.BanUser(decisionDTO.UtilisateurId);
                    await _mailService.NotifyUserStatusChangedAsync(
                        utilisateurSanctionne, 
                        utilisateurSanctionne.StatutId);
                    break;

                default:
                    return BadRequest("Type de décision non reconnu");
            }
            var createdDecision = await _decisionManager.AddAsync(decision);
            await _signalementManager.DeleteSignalementByUserId(decisionDTO.UtilisateurId);
            await _signalementManager.DeleteAsync(signalement);
            return CreatedAtAction(nameof(GetDecisionById), 
                new { id = createdDecision.DecisionId }, 
                new
                {
                    decisionId = createdDecision.DecisionId,
                    utilisateurId = createdDecision.UtilisateurId,
                    dateDecision = createdDecision.DecisionDate,
                    message = "Décision créée avec succès"
                });
        }
        catch (Exception e)
        {
            return BadRequest(new { 
                error = e.Message,
                innerError = e.InnerException?.Message ?? "Pas d'exception interne",
                stackTrace = e.StackTrace,
                type = e.GetType().Name
            });
        }
    }

    private async Task<ElementDecision> CreateElementDecision(ElementDecisionDTO dto)
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

            case ElementUtilisateurDTO utilisateur:
                elementDecision.ElementDecisionUtilisateur = new ElementDecisionUtilisateur();
                break;

            default:
                throw new ArgumentException("Type d'élément de décision non reconnu");
        }
        return elementDecision;
    }
}