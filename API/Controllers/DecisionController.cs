using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using API.Services;
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
    private readonly IAnnonceRepository<Annonce, int, FilterDTO> _annonceManager;
    private readonly INoteUtilisateurRepository _noteUtilisateurManager;
    private readonly IConversationRepository<Conversation, int> _conversationManager;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public DecisionController(
        IDecisionRepository decisionManager,
        ICurrentUserService currentUserService,
        IAnnonceRepository<Annonce, int, FilterDTO> annonceManager,
        INoteUtilisateurRepository noteUtilisateurManager,
        IConversationRepository<Conversation, int> conversationManager,
        IMapper mapper)
    {
        _currentUserService =  currentUserService;
        _decisionManager = decisionManager;
        _annonceManager = annonceManager;
        _noteUtilisateurManager = noteUtilisateurManager;
        _conversationManager = conversationManager;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Moderateur")]
    public async Task<ActionResult<ActionResult<DecisionDTO>>> GetAllDecisionsByModerateurId(int id)
    {
        IEnumerable<Decision> decisions = await _decisionManager.GetAllDecisionsByModerateurId(id);
        IEnumerable<DecisionDTO> decisionsDTO = _mapper.Map<IEnumerable<DecisionDTO>>(decisions);
        return Ok(decisionsDTO);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DecisionPostDTO>> CreateDecision([FromBody] DecisionPostDTO decisionDTO)
    {
        Console.WriteLine("-------------------------------------------------------- Fonction ----------------------------------------------------------");
        try
        {
            int? userId = await _currentUserService.GetUserId();
            if (userId == null)
            {
                return Unauthorized("Utilisateur non authentifié");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var decision = new Decision
            {
                ElementDecision = await CreateElementDecision(decisionDTO.ElementDecision),
                ModerateurId = userId.Value,
                UtilisateurId = decisionDTO.UtilisateurId,
                DecisionDate = DateTime.UtcNow
            };
            switch (decisionDTO)
            {
                case DecisionAvertissementPostDTO avertissement:
                    decision = await CreateAvertissement(decision);
                    break;

                case SanctionSuspensionPostDTO suspension:
                    decision = await CreateSanctionSuspension(decision, suspension);
                    break;

                case SanctionBannissementPostDTO bannissement:
                    decision = await CreateSanctionBannissement(decision, bannissement);
                    break;

                default:
                    return BadRequest("Type de décision non reconnu");
            }
            var createdDecision = await _decisionManager.AddAsync(decision);
            var resultDTO = _mapper.Map<DecisionPostDTO>(createdDecision);

            return CreatedAtAction(nameof(GetAllDecisionsByModerateurId), 
                new { id = userId.Value }, resultDTO);
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
        Console.WriteLine("-------------------------------------------------------- element ----------------------------------------------------------");
        var elementDecision = new ElementDecision();

        switch (dto)
        {
            case ElementDecisionAnnonceDTO annonce:
                Console.WriteLine("-------------------------------------------------------- appelle ----------------------------------------------------------");
                await _annonceManager.SuspendElement(annonce.AnnonceId);
                Console.WriteLine("-------------------------------------------------------- reception ----------------------------------------------------------");
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