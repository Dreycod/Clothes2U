using Shared.DTO.Abonnement;
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
    public class AbonnementController : ControllerBase
    {
        private readonly IAbonnementRepository<Abonnement, int> _abonnementRepo;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;

        public AbonnementController(IAbonnementRepository<Abonnement, int> repo, IMapper mapper, ICurrentUserService currentUserService)
        {
            _abonnementRepo = repo;
            _mapper = mapper;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Retourne la liste des utilisateurs que le follower dont on rentre l'id suit.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("suiveur/{id}")]
        public async Task<ActionResult<IEnumerable<AbonnementDetailDTO>>> GetAllUtilisateurSuiviByFollower(int id)
        {
            var result = await _abonnementRepo.GetAllUtilisateurSuiviByFollower(id);
            return Ok(_mapper.Map<IEnumerable<AbonnementDetailDTO>>(result));

        }

        /// <summary>
        /// Retourne la liste des followers suivant l'utilisateur dont on rentre l'id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("followers/{id}")]
        public async Task<ActionResult<IEnumerable<AbonnementDetailDTO>>> GetAllFollowersByUtilisateurSuivi(int id)
        {
            var result = await _abonnementRepo.GetAllFollowersByUtilisateurSuivi(id);
            return Ok(_mapper.Map<IEnumerable<AbonnementDetailDTO>>(result));

        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<AbonnementDTO>> Create([FromBody] int idUtilisateur)
        {
            int? connectedUserId = await _currentUserService.GetUserId();
            if (connectedUserId == null)
            {
                return Unauthorized();
            }
            bool exists = await _abonnementRepo.Exists((int)connectedUserId, idUtilisateur);

            if (exists)
                return BadRequest("Cet utilisateur est déjà suivi.");
            Abonnement abonnement = new Abonnement
            {
                UtilisateurSuiveurId = (int)connectedUserId,
                UtilisateurSuivisId = idUtilisateur
            };
            await _abonnementRepo.AddAsync(abonnement);
            return Ok(_mapper.Map<AbonnementDTO>(abonnement));
        }

        [Authorize]
        [HttpDelete("{idUtilisateur}")]
        public async Task<IActionResult> Delete(int idUtilisateur)
        {
            int? connectedUserId = await _currentUserService.GetUserId();
            if (connectedUserId == null)
            {
                return Unauthorized();
            }
            Abonnement abonnement = await _abonnementRepo.FindAbonnement((int)connectedUserId, idUtilisateur);
            if (abonnement == null)
            {
                return NotFound();
            }
            await _abonnementRepo.DeleteAsync(abonnement);
            return NoContent();
        }
    }
}
