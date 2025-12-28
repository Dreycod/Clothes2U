using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Managers;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.DemandeRestauration;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DemandeRestaurationController : ControllerBase
{
    private readonly IDemandeRestaurationRepository<DemandeRestauration, int>  _demandeRestaurationManager;
    private readonly IDecisionRepository _decisionManager;
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;

    public DemandeRestaurationController(
        IDemandeRestaurationRepository<DemandeRestauration, int> demandeRestaurationRepository,
        IDecisionRepository decisionManager,
        ICurrentUserService currentUserService,
        IMapper mapper)
    {
        _demandeRestaurationManager = demandeRestaurationRepository;
        _decisionManager = decisionManager;
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
        int? userId = await _currentUserService.GetUserId();
        if (userId == null)
        {
            return Unauthorized(new { message = "Utilisateur non authentifié" });
        }
    
        DemandeRestauration demande = await _demandeRestaurationManager
            .GetActiveDemandeRestaurationByUserId((int)userId);
        if (demande != null)
        {
            return BadRequest(new { message = "Une demande de restauration est déjà en cours de traitement." });
        }

        Decision decision = await _decisionManager.GetActiveDecisionByUserId((int)userId);
        if (decision == null)
        {
            return BadRequest(new { message = "Aucune sanction active trouvée. Vous ne pouvez pas faire de demande de restauration." });
        }
    
        DemandeRestauration demandeRestauration = new DemandeRestauration
        {
            Message = messageDemandeRestauration,
            DecisionId = decision.DecisionId
        };
    
        await _demandeRestaurationManager.AddAsync(demandeRestauration);
        DemandeRestaurationDetailDTO result = _mapper.Map<DemandeRestaurationDetailDTO>(demandeRestauration);
    
        return CreatedAtAction(nameof(GetDemandeRestauration), new { id = userId }, result);
    }
}