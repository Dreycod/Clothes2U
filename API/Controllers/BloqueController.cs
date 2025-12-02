using API.DTO.Bloque;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BloqueController : ControllerBase
    {
        private readonly IBloqueRepository<Bloque, int> _bloqueRepo;
        private readonly IMapper _mapper;

        public BloqueController(IBloqueRepository<Bloque, int> repo, IMapper mapper)
        {
            _bloqueRepo = repo;
            _mapper = mapper;
        }

        // Liste des utilisateurs que X a bloqué
        [HttpGet("bloqueur/{id}")]
        public async Task<ActionResult<IEnumerable<BloqueDetailDTO>>> GetByUtilisateurBloqueur(int id)
        {
            var result = await _bloqueRepo.GetByUtilisateurBloquantId(id);
            return Ok(_mapper.Map<IEnumerable<BloqueDetailDTO>>(result));
        }

        // Liste des utilisateurs qui ont bloqué X
        [HttpGet("bloque/{id}")]
        public async Task<ActionResult<IEnumerable<BloqueDetailDTO>>> GetByUtilisateurBloque(int id)
        {
            var result = await _bloqueRepo.GetByUtilisateurBloqueId(id);
            return Ok(_mapper.Map<IEnumerable<BloqueDetailDTO>>(result));
        }

        // Rechercher un utilisateur bloqué (par login)
        [HttpGet("search/{bloqueurId}")]
        public async Task<ActionResult<IEnumerable<BloqueDetailDTO>>> SearchByLogin(int bloqueurId, [FromQuery] string login)
        {
            var result = await _bloqueRepo.SearchBlockedByLogin(bloqueurId, login);
            return Ok(_mapper.Map<IEnumerable<BloqueDetailDTO>>(result));
        }

        // Vérifier si A bloque B
        [HttpGet("check")]
        public async Task<ActionResult<bool>> Check(int bloqueurId, int bloqueId)
        {
            bool exists = await _bloqueRepo.Exists(bloqueurId, bloqueId);
            return Ok(exists);
        }

        // Ajouter un blocage
        [HttpPost]
        public async Task<ActionResult<BloqueDTO>> Create(BloqueDTO dto)
        {
            bool exists = await _bloqueRepo.Exists(dto.BloqueurId, dto.UtilisateurBloqueId);

            if (exists)
                return BadRequest("Cet utilisateur est déjà bloqué.");

            var bloque = _mapper.Map<Bloque>(dto);
            await _bloqueRepo.AddAsync(bloque);

            return Ok(_mapper.Map<BloqueDTO>(bloque));
        }

        // 6️⃣ Supprimer un blocage
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _bloqueRepo.GetByIdAsync(id);
            if (entity == null)
                return NotFound();

            await _bloqueRepo.DeleteAsync(entity);
            return NoContent();
        }
    }
}
