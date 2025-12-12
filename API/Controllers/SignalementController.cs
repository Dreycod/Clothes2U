using API.DTO.Signalement;
using API.Models.EntityFramework;
using API.Models.Repository;
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
        private readonly IMapper _mapper;

        public SignalementController(ISignalementRepository repo, IMapper mapper)
        {
            _signalementManager = repo;
            _mapper = mapper;
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
        public async Task<ActionResult<SignalementDetailsDTO>> Create(SignalementCreateDTO dto)
        {
            var signalement = _mapper.Map<Signalement>(dto);

            var created = await _signalementManager.CreateWithRelationsAsync(
                signalement,
                dto.AnnonceId,
                dto.AvisId,
                dto.UtilisateurSignaleId
            );

            var result = _mapper.Map<SignalementDetailsDTO>(created);
            return CreatedAtAction(nameof(GetById), new { id = created.SignalementId }, result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var sig = await _signalementManager.GetByIdAsync(id);
            if (sig == null) return NotFound();
            await _signalementManager.DeleteAsync(sig);
            return NoContent();
        }
    }
}
