using Shared.DTO.Bloque;
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
    public class BloqueController : ControllerBase
    {
        private readonly IBloqueRepository<Bloque, int> _bloqueRepo;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IAbonnementRepository<Abonnement, int>  _abonnementManager;

        public BloqueController(
            IBloqueRepository<Bloque, int> repo,
            ICurrentUserService currentUserService,
            IAbonnementRepository<Abonnement, int>  abonnementManager,
            IMapper mapper)
        {
            _bloqueRepo = repo;
            _currentUserService = currentUserService;
            _abonnementManager = abonnementManager;
            _mapper = mapper;
        }
        [HttpGet("bloqueur/{id}")]
        public async Task<ActionResult<IEnumerable<BloqueDetailDTO>>> GetByUtilisateurBloqueur(int id)
        {
            var result = await _bloqueRepo.GetByUtilisateurBloquantId(id);
            return Ok(_mapper.Map<IEnumerable<BloqueDetailDTO>>(result));
        }
        [HttpGet("bloque/{id}")]
        public async Task<ActionResult<IEnumerable<BloqueDetailDTO>>> GetByUtilisateurBloque(int id)
        {
            var result = await _bloqueRepo.GetByUtilisateurBloqueId(id);
            return Ok(_mapper.Map<IEnumerable<BloqueDetailDTO>>(result));
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<BloqueDTO>> Create([FromBody] int utilisateurBloqueID)
        {
            int userId = await _currentUserService.GetUserIdOrThrow();
            bool exists = await _bloqueRepo.Exists(userId, utilisateurBloqueID);
            if (exists)
                return BadRequest("Cet utilisateur est déjà bloqué.");
            Abonnement abonnement = await _abonnementManager.FindAbonnement(userId, utilisateurBloqueID);
            if (abonnement == null)
            {
                await _abonnementManager.DeleteAsync(abonnement);
            }
            Bloque bloque = new Bloque()
            {
                UtilisateurBloqueurId = (int)userId,
                UtilisateurBloqueId = utilisateurBloqueID
            };
            await _bloqueRepo.AddAsync(bloque);

            return Ok(_mapper.Map<BloqueDTO>(bloque));
        }
        [HttpDelete("{utilisateurBloqueId}")]
        [Authorize]
        public async Task<IActionResult> Delete(int utilisateurBloqueId)
        {
            int userId = await _currentUserService.GetUserIdOrThrow();
            var entity = await _bloqueRepo.GetIfExists((int)userId, utilisateurBloqueId);
            if (entity == null)
                return NotFound();

            await _bloqueRepo.DeleteAsync(entity);
            return NoContent();
        }
    }
}
