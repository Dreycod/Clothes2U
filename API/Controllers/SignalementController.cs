using API.DTO.Signalement;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SignalementController : ControllerBase
    {
        private readonly ISignalementRepository _repo;
        private readonly IMapper _mapper;

        public SignalementController(ISignalementRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SignalementDetailDTO>> GetById(int id)
        {
            var sig = await _repo.GetByIdAsync(id);
            if (sig == null) return NotFound();

            return Ok(_mapper.Map<SignalementDetailDTO>(sig));
        }

        [HttpGet("Utilisateur/{id}")]
        public async Task<ActionResult<IEnumerable<SignalementDTO>>> GetByUtilisateur(int id)
        {
            var list = await _repo.GetByUtilisateurAsync(id);
            return Ok(_mapper.Map<IEnumerable<SignalementDTO>>(list));
        }

        [HttpGet("Type/{typeId}")]
        public async Task<ActionResult<IEnumerable<SignalementDTO>>> GetByType(int typeId)
        {
            var list = await _repo.GetByTypeAsync(typeId);
            return Ok(_mapper.Map<IEnumerable<SignalementDTO>>(list));
        }

        [HttpPost]
        public async Task<ActionResult<SignalementDetailDTO>> Create(SignalementCreateDTO dto)
        {
            var signalement = _mapper.Map<Signalement>(dto);

            var created = await _repo.CreateWithRelationsAsync(
                signalement,
                dto.AnnonceId,
                dto.AvisId,
                dto.UtilisateurSignaleId
            );

            var result = _mapper.Map<SignalementDetailDTO>(created);
            return CreatedAtAction(nameof(GetById), new { id = created.SignalementId }, result);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var sig = await _repo.GetByIdAsync(id);
            if (sig == null) return NotFound();

            await _repo.DeleteAsync(sig);
            return NoContent();
        }
    }
}
