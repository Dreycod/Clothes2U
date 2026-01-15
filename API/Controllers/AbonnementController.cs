using Shared.DTO.Abonnement;
using Shared.DTO.Bloque;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO.Utilisateur;

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
        /// Retourne la liste des utilisateurs que l'utilisateur connecté suit (ses abonnements).
        /// </summary>
        /// <returns>La liste des utilisateurs suivis par l'utilisateur connecté.</returns>
        /// <response code="200">Retourne la liste des abonnements de l'utilisateur connecté.</response>
        /// <response code="401">L'utilisateur n'est pas authentifié.</response>
        /// <response code="500">Erreur interne du serveur.</response>
        [Authorize]
        [HttpGet("abonnements")]
        [ProducesResponseType(typeof(IEnumerable<UtilisateurCardDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<UtilisateurCardDTO>>> GetAbonnements()
        {
            int userId = await _currentUserService.GetUserIdOrThrow();
            var abonnements = await _abonnementRepo.GetAllUtilisateurSuiviByFollower(userId);
            var utilisateursSuivis = abonnements.Select(a => a.UtilisateurSuivis);
            return Ok(_mapper.Map<IEnumerable<UtilisateurCardDTO>>(utilisateursSuivis));
        }
        

        /// <summary>
        /// Retourne la liste des utilisateurs suivis par un utilisateur spécifique.
        /// </summary>
        /// <param name="id">L'identifiant de l'utilisateur dont on veut récupérer les abonnements.</param>
        /// <returns>La liste des utilisateurs suivis par l'utilisateur spécifié.</returns>
        /// <response code="200">Retourne la liste des utilisateurs suivis.</response>
        /// <response code="404">L'utilisateur spécifié n'existe pas.</response>
        /// <response code="500">Erreur interne du serveur.</response>
        [HttpGet("suiveur/{id}")]
        [ProducesResponseType(typeof(IEnumerable<AbonnementDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<AbonnementDetailDTO>>> GetAllUtilisateurSuiviByFollower(int id)
        {
            var result = await _abonnementRepo.GetAllUtilisateurSuiviByFollower(id);
            return Ok(_mapper.Map<IEnumerable<AbonnementDetailDTO>>(result));

        }

        /// <summary>
        /// Retourne la liste des followers (abonnés) d'un utilisateur spécifique.
        /// </summary>
        /// <param name="id">L'identifiant de l'utilisateur dont on veut récupérer les followers.</param>
        /// <returns>La liste des followers de l'utilisateur spécifié.</returns>
        /// <response code="200">Retourne la liste des followers.</response>
        /// <response code="404">L'utilisateur spécifié n'existe pas.</response>
        /// <response code="500">Erreur interne du serveur.</response>
        [HttpGet("followers/{id}")]
        [ProducesResponseType(typeof(IEnumerable<AbonnementDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        
        public async Task<ActionResult<IEnumerable<AbonnementDetailDTO>>> GetAllFollowersByUtilisateurSuivi(int id)
        {
            var result = await _abonnementRepo.GetAllFollowersByUtilisateurSuivi(id);
            return Ok(_mapper.Map<IEnumerable<AbonnementDetailDTO>>(result));

        }
        /// <summary>
        /// Crée un nouvel abonnement pour que l'utilisateur connecté suive un autre utilisateur.
        /// </summary>
        /// <param name="idUtilisateur">L'identifiant de l'utilisateur à suivre (dans le corps de la requête).</param>
        /// <returns>L'abonnement créé.</returns>
        /// <response code="200">L'abonnement a été créé avec succès.</response>
        /// <response code="400">L'abonnement existe déjà ou l'utilisateur tente de se suivre lui-même.</response>
        /// <response code="401">L'utilisateur n'est pas authentifié.</response>
        /// <response code="500">Erreur interne du serveur.</response>
        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(AbonnementDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<AbonnementDTO>> Create([FromBody] int idUtilisateur)
        {
            int userId = await _currentUserService.GetUserIdOrThrow();
            bool exists = await _abonnementRepo.Exists(userId, idUtilisateur);

            if (exists)
                return BadRequest("Cet utilisateur est déjà suivi.");
            if (userId == idUtilisateur)
                return BadRequest();
            Abonnement abonnement = new Abonnement
            {
                UtilisateurSuiveurId = userId,
                UtilisateurSuivisId = idUtilisateur
            };
            await _abonnementRepo.AddAsync(abonnement);
            return Ok(_mapper.Map<AbonnementDTO>(abonnement));
        }
        /// <summary>
        /// Supprime un abonnement existant (l'utilisateur connecté arrête de suivre un autre utilisateur).
        /// </summary>
        /// <param name="idUtilisateur">L'identifiant de l'utilisateur à ne plus suivre.</param>
        /// <returns>Aucun contenu en cas de succès.</returns>
        /// <response code="204">L'abonnement a été supprimé avec succès.</response>
        /// <response code="404">L'abonnement n'existe pas.</response>
        /// <response code="401">L'utilisateur n'est pas authentifié.</response>
        /// <response code="500">Erreur interne du serveur.</response>
        [Authorize]
        [HttpDelete("{idUtilisateur}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int idUtilisateur)
        {
            int userId = await _currentUserService.GetUserIdOrThrow();
            Abonnement abonnement = await _abonnementRepo.FindAbonnement(userId, idUtilisateur);
            if (abonnement == null)
            {
                return NotFound();
            }
            await _abonnementRepo.DeleteAsync(abonnement);
            return NoContent();
        }
    }
}
