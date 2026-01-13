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
        Console.WriteLine("=== CreateDecision START ===");
        Console.WriteLine($"Type reçu: {decisionDTO?.GetType().Name}");
        
        Signalement signalement = await _signalementManager.GetByIdAsync(decisionDTO.SignalementId);
        
        if (signalement == null)
        {
            Console.WriteLine("❌ Signalement introuvable");
            return NotFound("Signalement introuvable");
        }
        Console.WriteLine($"✅ Signalement: {signalement.SignalementId}");

        int userId = await _currentUserService.GetUserIdOrThrow();
        Console.WriteLine($"✅ UserId: {userId}");
        
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        
        Utilisateur utilisateurSanctionne = await _utilisateurRepository.GetByIdAsync(decisionDTO.UtilisateurId);
        if (utilisateurSanctionne == null)
        {
            Console.WriteLine("❌ Utilisateur introuvable");
            return NotFound("Utilisateur introuvable");
        }
        Console.WriteLine($"✅ Utilisateur: {utilisateurSanctionne.Login}");
        
        Console.WriteLine("🔨 CreateElementDecision...");
        var elementDecision = await CreateElementDecision(decisionDTO.ElementDecision);
        Console.WriteLine($"✅ ElementDecision créé");
        
        var decision = new Decision
        {
            ElementDecision = elementDecision,
            ModerateurId = userId,
            UtilisateurId = decisionDTO.UtilisateurId,
            DecisionDate = DateTime.UtcNow
        };
        Console.WriteLine("✅ Decision objet créé");
        
        switch (decisionDTO)
        {
            case DecisionAvertissementPostDTO avertissement:
                Console.WriteLine("➡️ DecisionAvertissementPostDTO");
                decision.DecisionAvertissement = new DecisionAvertissement();
                await _notificationService.CreateNotificationAvertissement(
                    avertissement.UtilisateurId,
                    avertissement.MessageModerateur);
                break;

            case SanctionSuspensionPostDTO suspension:
                Console.WriteLine("➡️ SanctionSuspensionPostDTO");
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
                Console.WriteLine("➡️ SanctionBannissementPostDTO");
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
                Console.WriteLine($"❌ DEFAULT CASE! Type: {decisionDTO.GetType().Name}");
                return BadRequest("Type de décision non reconnu");
        }
        
        // ✅✅✅ SAUVEGARDER LA DECISION D'ABORD ✅✅✅
        Console.WriteLine("💾 APPEL AddAsync...");
        var createdDecision = await _decisionManager.AddAsync(decision);
        Console.WriteLine($"✅✅✅ DECISION CRÉÉE ! ID: {createdDecision.DecisionId}");
        
        // ✅✅✅ PUIS SUPPRIMER LES SIGNALEMENTS ✅✅✅
        Console.WriteLine("🗑️ Suppression signalements...");
        await _signalementManager.DeleteSignalementByUserId(decisionDTO.UtilisateurId);
        await _signalementManager.DeleteAsync(signalement);
        Console.WriteLine("✅ Signalements supprimés");
        
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
        Console.WriteLine($"❌❌❌ EXCEPTION CreateDecision: {e.Message}");
        Console.WriteLine($"Inner: {e.InnerException?.Message}");
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