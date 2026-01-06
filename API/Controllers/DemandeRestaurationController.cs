using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.DTO.DemandeRestauration;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DemandeRestaurationController : ControllerBase
{
    private readonly IDemandeRestaurationRepository<DemandeRestauration, int>  _demandeRestaurationManager;
    private readonly IUtilisateurRepository _utilisateurManager;
    private readonly IDecisionRepository _decisionManager;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public DemandeRestaurationController(
        IDemandeRestaurationRepository<DemandeRestauration, int> demandeRestaurationRepository,
        IUtilisateurRepository utilisateurRepository,
        IDecisionRepository decisionManager,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _demandeRestaurationManager = demandeRestaurationRepository;
        _decisionManager = decisionManager;
        _utilisateurManager = utilisateurRepository;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Moderateur")]
    public async Task<ActionResult<IEnumerable<DemandeRestaurationDTO>>> GetAllDemandes()
    {
        IEnumerable<DemandeRestauration> demandes = await _demandeRestaurationManager.GetAllAsync();
        IEnumerable<DemandeRestaurationDTO> demandesDTO = _mapper.Map<IEnumerable<DemandeRestaurationDTO>>(demandes);
        return Ok(demandesDTO);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Moderateur")]
    public async Task<ActionResult<DemandeRestaurationDetailDTO>> GetDemandeRestauration(int id)
    {
        DemandeRestauration demande = await _demandeRestaurationManager.GetByIdAsync(id);
        if (demande == null)
        {
            return NotFound();
        }
        DemandeRestaurationDetailDTO demandeDTO = _mapper.Map<DemandeRestaurationDetailDTO>(demande);
        return Ok(demandeDTO);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<DemandeRestaurationDetailDTO>> CreateDemandeRestauration(
        [FromBody] string messageDemandeRestauration)
    {
        int userId = await _currentUserService.GetUserIdOrThrow();
        DemandeRestauration demande = await _demandeRestaurationManager
            .GetActiveDemandeRestaurationByUserId((int)userId);
        if (demande != null)
        {
            if (demande.Status == "En cours")
            {
                return BadRequest(new { message = "Une demande de restauration est déjà en cours de traitement." });
            }
            if (demande.Status == "Refusée")
            {
                return BadRequest(new { message = "Vous avez déjà soumis une demande qui a été refusée." });
            }

            return BadRequest();
        }

        Decision decision = await _decisionManager.GetActiveDecisionByUserId((int)userId);
        if (decision == null)
        {
            return BadRequest(new { message = "Aucune sanction active trouvée. Vous ne pouvez pas faire de demande de restauration." });
        }
    
        DemandeRestauration demandeRestauration = new DemandeRestauration
        {
            Status = "En cours",
            Message = messageDemandeRestauration,
            DecisionId = decision.DecisionId,
            Date = DateTime.UtcNow
        };
    
        await _demandeRestaurationManager.AddAsync(demandeRestauration);
        DemandeRestaurationDetailDTO result = _mapper.Map<DemandeRestaurationDetailDTO>(demandeRestauration);
    
        return CreatedAtAction(nameof(GetDemandeRestauration), new { id = userId }, result);
    }
    
    [HttpPost("decisionDemandeRestauration")]
    [Authorize(Roles = "Admin,Moderateur")]
    public async Task<IActionResult> SubmitDecisionDemande([FromBody] DecisionDemandeRestaurationDTO decisionDemandeRestauration)
    {
        var demande = await _demandeRestaurationManager.GetByIdAsync(decisionDemandeRestauration.DemandeId);
        if (demande == null)
        {
            return NotFound($"Demande de restauration {decisionDemandeRestauration.DemandeId} introuvable.");
        }
        if (demande.Status != "En cours")
        {
            return BadRequest("Cette demande a déjà été traitée.");
        }
        try
        {
            if (decisionDemandeRestauration.IsRestored)
            {
                var utilisateur = await _utilisateurManager.GetByIdAsync(decisionDemandeRestauration.UtilisateurId);
                if (utilisateur == null)
                {
                    return NotFound($"Utilisateur {decisionDemandeRestauration.UtilisateurId} introuvable.");
                }
                utilisateur.StatutId = 1;
                await _utilisateurManager.UpdateAsync(utilisateur);
                demande.Status = "Accepté";
                await _demandeRestaurationManager.UpdateAsync(demande);
            }
            else
            {
                demande.Status = "Refusée";
                await _demandeRestaurationManager.UpdateAsync(demande);
            }
        
            return NoContent(); 
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Erreur: {ex.Message}");
        }
    }
}