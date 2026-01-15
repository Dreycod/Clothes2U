using API.Services;
using API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Decision;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DecisionController : ControllerBase
{
    private readonly IDecisionService _decisionService;
    private readonly ICurrentUserService _currentUserService;

    public DecisionController(
        IDecisionService decisionService,
        ICurrentUserService currentUserService)
    {
        _decisionService = decisionService;
        _currentUserService = currentUserService;
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
    public async Task<ActionResult<IEnumerable<DecisionDTO>>> GetAllDecisionsByModerateurId()
    {
        int userId = await _currentUserService.GetUserIdOrThrow();
        var decisionsDTO = await _decisionService.GetAllDecisionsByModeratorAsync(userId);
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
        var decisionDTO = await _decisionService.GetDecisionByIdAsync(id);
        
        if (decisionDTO == null)
        {
            return NotFound();
        }

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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            int userId = await _currentUserService.GetUserIdOrThrow();
            var createdDecision = await _decisionService.CreateDecisionAsync(decisionDTO, userId);

            return CreatedAtAction(
                nameof(GetDecisionById),
                new { id = createdDecision.DecisionId },
                new
                {
                    decisionId = createdDecision.DecisionId,
                    utilisateurId = createdDecision.UtilisateurId,
                    dateDecision = createdDecision.DecisionDate,
                    message = "Décision créée avec succès"
                });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                error = ex.Message,
                innerError = ex.InnerException?.Message ?? "Pas d'exception interne",
                stackTrace = ex.StackTrace,
                type = ex.GetType().Name
            });
        }
    }
}