using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.DTO.DemandeRestauration;
using Shared.Enums;

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
    /// <summary>
    /// Récupère la liste de toutes les demandes de restauration.
    /// </summary>
    /// <returns>Une collection de demandes de restauration.</returns>
    /// <response code="200">Retourne la liste des demandes de restauration.</response>
    /// <response code="401">Non autorisé - authentification requise.</response>
    /// <response code="403">Accès refusé - rôle Admin ou Moderateur requis.</response>
    [HttpGet]
    [Authorize(Roles = "Admin,Moderateur")]
    [ProducesResponseType(typeof(IEnumerable<DemandeRestaurationDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<IEnumerable<DemandeRestaurationDTO>>> GetAllDemandes()
    {
        IEnumerable<DemandeRestauration> demandes = await _demandeRestaurationManager.GetAllAsync();
        IEnumerable<DemandeRestaurationDTO> demandesDTO = _mapper.Map<IEnumerable<DemandeRestaurationDTO>>(demandes);
        return Ok(demandesDTO);
    }
    /// <summary>
    /// Récupère les détails d'une demande de restauration spécifique.
    /// </summary>
    /// <param name="id">L'identifiant de la demande de restauration.</param>
    /// <returns>Les détails de la demande de restauration.</returns>
    /// <response code="200">Retourne les détails de la demande.</response>
    /// <response code="401">Non autorisé - authentification requise.</response>
    /// <response code="403">Accès refusé - rôle Admin ou Moderateur requis.</response>
    /// <response code="404">Demande de restauration introuvable.</response>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Moderateur")]
    [ProducesResponseType(typeof(DemandeRestaurationDetailDTO), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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
    /// <summary>
    /// Crée une nouvelle demande de restauration pour l'utilisateur connecté.
    /// </summary>
    /// <param name="messageDemandeRestauration">Le message de justification de la demande.</param>
    /// <returns>Les détails de la demande créée.</returns>
    /// <response code="201">Demande de restauration créée avec succès.</response>
    /// <response code="400">Requête invalide - demande déjà en cours, refusée ou aucune sanction active.</response>
    /// <response code="401">Non autorisé - authentification requise.</response>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(DemandeRestaurationDetailDTO), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<DemandeRestaurationDetailDTO>> CreateDemandeRestauration(
        [FromBody] string messageDemandeRestauration)
    {
        int userId = await _currentUserService.GetUserIdOrThrow();
        DemandeRestauration demande = await _demandeRestaurationManager
            .GetActiveDemandeRestaurationByUserId(userId);
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
            Date = DateTime.UtcNow.ToLocalTime()
        };
    
        await _demandeRestaurationManager.AddAsync(demandeRestauration);
        DemandeRestaurationDetailDTO result = _mapper.Map<DemandeRestaurationDetailDTO>(demandeRestauration);
    
        return CreatedAtAction(nameof(GetDemandeRestauration), new { id = userId }, result);
    }
    /// <summary>
    /// Traite une demande de restauration en l'acceptant ou la refusant.
    /// </summary>
    /// <param name="decisionDemandeRestauration">Les informations de la décision (acceptation ou refus).</param>
    /// <returns>Aucun contenu en cas de succès.</returns>
    /// <response code="204">Décision traitée avec succès.</response>
    /// <response code="400">Requête invalide - demande déjà traitée.</response>
    /// <response code="401">Non autorisé - authentification requise.</response>
    /// <response code="403">Accès refusé - rôle Admin ou Moderateur requis.</response>
    /// <response code="404">Demande de restauration ou utilisateur introuvable.</response>
    /// <response code="500">Erreur serveur interne.</response>
    [HttpPost("decisionDemandeRestauration")]
    [Authorize(Roles = "Admin,Moderateur")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> SubmitDecisionDemande([FromBody] DecisionDemandeRestaurationDTO decisionDemandeRestauration)
    {
        DemandeRestauration demande = await _demandeRestaurationManager.GetByIdAsync(decisionDemandeRestauration.DemandeId);
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

                Decision decision = await _decisionManager.GetByIdAsync(demande.DecisionId);
                decision.DecisionSanction.EstEnCours = false;
                await _decisionManager.UpdateAsync(decision);
                utilisateur.StatutId = (int)UtilisateurStatut.Actif;
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