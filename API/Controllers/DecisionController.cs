using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Decision;

namespace API.Controllers;




[ApiController]
[Route("api/[controller]")]
public class DecisionController : ControllerBase
{
    private readonly IDecisionRepository _decisionManager;
    private readonly IUtilisateurRepository _utilisateurManager;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public DecisionController(IDecisionRepository decisionManager,ICurrentUserService currentUserService,IUtilisateurRepository utilisateurManager, IMapper mapper)
    {
        _currentUserService =  currentUserService;
        _utilisateurManager =  utilisateurManager;
        _decisionManager = decisionManager;
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
    [Authorize(Roles = "Admin,Moderateur")]
    public async Task<ActionResult<DecisionPostDTO>> CreateDecision(DecisionPostDTO decisionDTO)
    {
        try
        {
            int? userId = await _currentUserService.GetUserId();
            if (userId == null)
            {
                return Unauthorized("Utilisateur non authentifié");
            }

            Utilisateur user = await _utilisateurManager.GetByIdAsync((int)userId);
            if (user.Role.RoleUtilisateurLibelle != "Moderateur")
            {
                return Unauthorized("Utilisateur non moderateur");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var decision = new Decision
            {
                ModerateurId = userId.Value,
                UtilisateurId = decisionDTO.UtlisateurId,
                DecisionDate = decisionDTO.DateDecision
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
        var elementDecision = await CreateElementDecision(dto.ElementDecision);

        var sanction = new DecisionSanction
        {
            EstEnCours = true,
            ElementDecision = elementDecision
        };

        sanction.SanctionSuspension = new SanctionSuspension
        {
            DateFinSuspension = dto.DateFinSuspension,
            DecisionSanction = sanction
        };

        elementDecision.DecisionSanction = sanction;
        decision.DecisionSanction = sanction;

        return decision;
    }

    private async Task<Decision> CreateSanctionBannissement(Decision decision, SanctionBannissementPostDTO dto)
    {
        var elementDecision = await CreateElementDecision(dto.ElementDecision);

        var sanction = new DecisionSanction
        {
            EstEnCours = true,
            ElementDecision = elementDecision
        };

        sanction.SanctionBannissement = new SanctionBannissement
        {
            DecisionSanction = sanction
        };

        elementDecision.DecisionSanction = sanction;
        decision.DecisionSanction = sanction;

        return decision;
    }

    private async Task<ElementDecision> CreateElementDecision(ElementDecisionDTO dto)
    {
        var elementDecision = new ElementDecision();

        switch (dto)
        {
            case ElementDecisionAnnonceDTO annonce:
                elementDecision.ElementDecisionAnnonce = new ElementDecisionAnnonce
                {
                    AnnonceId = annonce.AnnonceId
                };
                break;

            case ElementDecisionMessageDTO message:
                elementDecision.ElementDecisionMessage = new ElementDecisionMessage
                {
                    MessageId = message.MessageId
                };
                break;

            case ElementAvisDTO avis:
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