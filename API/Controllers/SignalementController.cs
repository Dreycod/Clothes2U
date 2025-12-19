using Shared.DTO.Signalement;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignalementController : ControllerBase
    {
        private readonly ISignalementRepository _signalementManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public SignalementController(
            ISignalementRepository repo,
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _signalementManager = repo;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SignalementDetailsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id)
        {
            var sig = await _signalementManager.GetByIdAsync(id);
            if (sig == null)
            {
                return NotFound();
            }

            var result = _mapper.Map<SignalementDetailsDTO>(sig);
            return new ObjectResult(result)
            {
                StatusCode = StatusCodes.Status200OK,
                DeclaredType = typeof(SignalementDetailsDTO)
            };
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SignalementDTO>>> GetAll()
        {
            var signalements = await _signalementManager.GetAllAsync();
            IEnumerable<SignalementDTO> signalementsDTO = _mapper.Map<IEnumerable<SignalementDTO>>(signalements);
            return Ok(signalementsDTO);
        }

        [HttpGet("Utilisateur/{id}")]
        public async Task<ActionResult<IEnumerable<SignalementDTO>>> GetByUtilisateur(int id)
        {
            var list = await _signalementManager.GetByUtilisateurAsync(id);
            return Ok(_mapper.Map<IEnumerable<SignalementDTO>>(list));
        }

        [HttpGet("Type/{typeId}")]
        public async Task<ActionResult<IEnumerable<SignalementDTO>>> GetByType(int typeId)
        {
            var list = await _signalementManager.GetByTypeAsync(typeId);
            return Ok(_mapper.Map<IEnumerable<SignalementDTO>>(list));
        }
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(SignalementDetailsDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] SignalementCreateDTO dto)
        {
            try
            {
                int? userId = await _currentUserService.GetUserId();
                if (userId == null)
                {
                    return Unauthorized();
                }

                var signalement = _mapper.Map<Signalement>(dto);
                signalement.UtilisateurId = (int)userId;
                int? annonceId = (dto as SignalementAnnonceCreateDTO)?.AnnonceSignaleeId;
                int? avisId = (dto as SignalementAvisCreateDTO)?.AvisId;
                int? utilisateurSignaleId = (dto as SignalementUtilisateurCreateDTO)?.UtilisateurSignaleId;
                var createdSignalement = await _signalementManager.CreateWithRelationsAsync(
                    signalement,
                    annonceId,
                    avisId,
                    utilisateurSignaleId
                );
                var result = _mapper.Map<SignalementDetailsDTO>(createdSignalement);

                return CreatedAtAction(
                    nameof(GetById),
                    new { id = createdSignalement.SignalementId },
                    result
                );
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

       
    }
}
