using API.DTO.Abonnement;
using API.DTO.Bloque;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AbonnementController : ControllerBase
    {
        private readonly IAbonnementRepository<Abonnement, int> _abonnementRepo;
        private readonly IMapper _mapper;

        public AbonnementController(IAbonnementRepository<Abonnement, int> repo, IMapper mapper)
        {
            _abonnementRepo = repo;
            _mapper = mapper;
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

        [HttpPost]
        public async Task<ActionResult<BloqueDTO>> Create(AbonnementDTO dto)
        {
            bool exists = await _abonnementRepo.Exists(dto.UtilisateurSuiveurId, dto.UtilisateurSuiviId);

            if (exists)
                return BadRequest("Cet utilisateur est déjà suivi.");

            var abo = _mapper.Map<Abonnement>(dto);
            await _abonnementRepo.AddAsync(abo);

            return Ok(_mapper.Map<AbonnementDTO>(abo));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _abonnementRepo.GetByIdAsync(id);
            if (entity == null)
                return NotFound();

            await _abonnementRepo.DeleteAsync(entity);
            return NoContent();
        }
    }
}
