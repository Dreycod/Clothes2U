using Shared.DTO.Signalement;
using API.Models.Repository;
using API.Services;
using API.Services.Interfaces;
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
        private readonly ISignalementService _signalementService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public SignalementController(
            ISignalementRepository repo,
            ISignalementService signalementService,
            IMapper mapper,
            ICurrentUserService currentUserService)
        {
            _signalementManager = repo;
            _signalementService = signalementService;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }
        /// <summary>
        /// Récupère un signalement par son identifiant.
        /// </summary>
        /// <param name="id">L'identifiant du signalement.</param>
        /// <returns>Les détails du signalement.</returns>
        /// <response code="200">Retourne les détails du signalement.</response>
        /// <response code="404">Le signalement n'existe pas.</response>
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
        /// <summary>
        /// Récupère tous les signalements.
        /// </summary>
        /// <returns>La liste de tous les signalements.</returns>
        /// <response code="200">Retourne la liste des signalements.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SignalementDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SignalementDTO>>> GetAll()
        {
            var signalements = await _signalementManager.GetAllAsync();
            IEnumerable<SignalementDTO> signalementsDTO = _mapper.Map<IEnumerable<SignalementDTO>>(signalements);
            return Ok(signalementsDTO);
        }
        /// <summary>
        /// Récupère tous les signalements d'un utilisateur spécifique.
        /// </summary>
        /// <param name="id">L'identifiant de l'utilisateur.</param>
        /// <returns>La liste des signalements de l'utilisateur.</returns>
        /// <response code="200">Retourne la liste des signalements de l'utilisateur.</response>
        [HttpGet("Utilisateur/{id}")]
        [ProducesResponseType(typeof(IEnumerable<SignalementDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SignalementDTO>>> GetByUtilisateur(int id)
        {
            var list = await _signalementManager.GetByUtilisateurAsync(id);
            return Ok(_mapper.Map<IEnumerable<SignalementDTO>>(list));
        }
        /// <summary>
        /// Récupère tous les signalements d'un type spécifique.
        /// </summary>
        /// <param name="typeId">L'identifiant du type de signalement.</param>
        /// <returns>La liste des signalements du type spécifié.</returns>
        /// <response code="200">Retourne la liste des signalements du type spécifié.</response>
        [HttpGet("Type/{typeId}")]
        
        [ProducesResponseType(typeof(IEnumerable<SignalementDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SignalementDTO>>> GetByType(int typeId)
        {
            var list = await _signalementManager.GetByTypeAsync(typeId);
            return Ok(_mapper.Map<IEnumerable<SignalementDTO>>(list));
        }
        /// <summary>
        /// Crée un nouveau signalement.
        /// Le type de signalement peut être : annonce, avis ou utilisateur.
        /// </summary>
        /// <param name="dto">Les données du signalement à créer.</param>
        /// <returns>Le signalement créé.</returns>
        /// <response code="201">Le signalement a été créé avec succès.</response>
        /// <response code="400">Les données sont invalides.</response>
        /// <response code="401">L'utilisateur n'est pas authentifié.</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(SignalementDetailsDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] SignalementCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                int userId = await _currentUserService.GetUserIdOrThrow();
                var result = await _signalementService.CreateSignalementAsync(dto, userId);

                return new ObjectResult(result)
                {
                    StatusCode = StatusCodes.Status201Created,
                    DeclaredType = typeof(SignalementDetailsDTO)
                };
            }
            catch (Exception ex)
            {
                return BadRequest(new 
                { 
                    error = ex.Message,
                    type = ex.GetType().Name
                });
            }
        }

    }
}
