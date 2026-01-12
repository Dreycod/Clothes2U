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

    [HttpGet]
    [Authorize(Roles = "Admin,Moderateur")]
    public async Task<ActionResult<ActionResult<DecisionDTO>>> GetAllDecisionsByModerateurId()
    {
        int userId = await _currentUserService.GetUserIdOrThrow();
        IEnumerable<Decision> decisions = await _decisionManager.GetAllDecisionsByModerateurId((int)userId);
        IEnumerable<DecisionDTO> decisionsDTO = _mapper.Map<IEnumerable<DecisionDTO>>(decisions);
        return Ok(decisionsDTO);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DecisionSuspensionDetailDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(DecisionBannissementDetailDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
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

    [HttpPost]
    [Authorize(Roles = "Admin,Moderateur")]
    public async Task<ActionResult<DecisionPostDTO>> CreateDecision([FromBody] DecisionPostDTO decisionDTO)
    {
        try
        {
            Signalement signalement = await _signalementManager.GetByIdAsync(decisionDTO.SignalementId);
            int userId = await _currentUserService.GetUserIdOrThrow();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (decisionDTO is DecisionIgnorPostDTO)
            {
                await _signalementManager.DeleteAsync(signalement);
                return decisionDTO;
            }

            Utilisateur utilisateurSanctionne = await _utilisateurRepository.GetByIdAsync(decisionDTO.UtilisateurId);
            var decision = new Decision
            {
                ElementDecision = await CreateElementDecision(decisionDTO.ElementDecision),
                ModerateurId = userId,
                UtilisateurId = decisionDTO.UtilisateurId,
                DecisionDate = DateTime.UtcNow
            };
            switch (decisionDTO)
            {
                case DecisionAvertissementPostDTO avertissement:
                    await _notificationService.CreateNotificationAvertissement(avertissement.UtilisateurId,
                        avertissement.MessageModerateur);
                    await _signalementManager.DeleteSignalementByUserId(decisionDTO.UtilisateurId);
                    decision = await CreateAvertissement(decision); 
                    break;

                case SanctionSuspensionPostDTO suspension:
                    await _utilisateurRepository.SuspendUser(decisionDTO.UtilisateurId);
                    await _signalementManager.DeleteSignalementByUserId(decisionDTO.UtilisateurId);
                    decision = await CreateSanctionSuspension(decision, suspension);
                    await _mailService.NotifyUserStatusChangedAsync(utilisateurSanctionne, utilisateurSanctionne.StatutId);
                    break;

                case SanctionBannissementPostDTO bannissement:
                    await _utilisateurRepository.BanUser(decisionDTO.UtilisateurId);
                    await _signalementManager.DeleteSignalementByUserId(decisionDTO.UtilisateurId);
                    decision = await CreateSanctionBannissement(decision, bannissement);
                    await _mailService.NotifyUserStatusChangedAsync(utilisateurSanctionne, utilisateurSanctionne.StatutId);
                    break;
                default:
                    return BadRequest("Type de décision non reconnu");
            }
            await _signalementManager.DeleteAsync(signalement);
            var createdDecision = await _decisionManager.AddAsync(decision);
            var resultDTO = _mapper.Map<DecisionPostDTO>(createdDecision);
            return CreatedAtAction(nameof(GetAllDecisionsByModerateurId), 
                new { id = userId }, resultDTO);
        
        }
        catch (Exception e)
        {
            var innerException = e.InnerException?.Message ?? "Pas d'exception interne";
            return BadRequest(new { 
                error = e.Message,
                innerError = innerException,
                stackTrace = e.StackTrace
            });
        }
    }

    private async Task<Decision> CreateAvertissement(Decision decision)
    {
        decision.DecisionAvertissement = new DecisionAvertissement();

        return decision;
    }

    private async Task<Decision> CreateSanctionSuspension(Decision decision, SanctionSuspensionPostDTO dto)
    {
        var sanction = new DecisionSanction
        {
            EstEnCours = true,
        };

        sanction.SanctionSuspension = new SanctionSuspension
        {
            DateFinSuspension = dto.DateFinSuspension,
            DecisionSanction = sanction
        };
        decision.DecisionSanction = sanction;

        return decision;
    }

    private async Task<Decision> CreateSanctionBannissement(Decision decision, SanctionBannissementPostDTO dto)
    {
        var sanction = new DecisionSanction
        {
            EstEnCours = true,
        };

        sanction.SanctionBannissement = new SanctionBannissement
        {
            DecisionSanction = sanction
        };
        decision.DecisionSanction = sanction;
        return decision;
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