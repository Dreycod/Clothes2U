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
        /// <summary>
        /// Récupère la liste des utilisateurs bloqués par un utilisateur spécifique.
        /// </summary>
        /// <param name="id">L'identifiant de l'utilisateur bloqueur.</param>
        /// <returns>La liste des utilisateurs bloqués par cet utilisateur.</returns>
        /// <response code="200">Retourne la liste des utilisateurs bloqués.</response>
        /// <response code="404">L'utilisateur spécifié n'existe pas.</response>
        /// <response code="500">Erreur interne du serveur.</response>
        [HttpGet("bloqueur/{id}")]
        [ProducesResponseType(typeof(IEnumerable<BloqueDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<BloqueDetailDTO>>> GetByUtilisateurBloqueur(int id)
        {
            var result = await _bloqueRepo.GetByUtilisateurBloquantId(id);
            return Ok(_mapper.Map<IEnumerable<BloqueDetailDTO>>(result));
        }
        /// <summary>
        /// Récupère la liste des utilisateurs qui ont bloqué un utilisateur spécifique.
        /// </summary>
        /// <param name="id">L'identifiant de l'utilisateur bloqué.</param>
        /// <returns>La liste des utilisateurs qui ont bloqué cet utilisateur.</returns>
        /// <response code="200">Retourne la liste des bloqueurs.</response>
        /// <response code="404">L'utilisateur spécifié n'existe pas.</response>
        /// <response code="500">Erreur interne du serveur.</response>
        [HttpGet("bloque/{id}")]
        [ProducesResponseType(typeof(IEnumerable<BloqueDetailDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<BloqueDetailDTO>>> GetByUtilisateurBloque(int id)
        {
            var result = await _bloqueRepo.GetByUtilisateurBloqueId(id);
            return Ok(_mapper.Map<IEnumerable<BloqueDetailDTO>>(result));
        }
        /// <summary>
        /// Bloque un utilisateur.
        /// Si l'utilisateur connecté suit l'utilisateur à bloquer, l'abonnement est automatiquement supprimé.
        /// </summary>
        /// <param name="utilisateurBloqueID">L'identifiant de l'utilisateur à bloquer (dans le corps de la requête).</param>
        /// <returns>Le blocage créé.</returns>
        /// <response code="200">L'utilisateur a été bloqué avec succès.</response>
        /// <response code="400">L'utilisateur est déjà bloqué.</response>
        /// <response code="401">L'utilisateur n'est pas authentifié.</response>
        /// <response code="500">Erreur interne du serveur.</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(BloqueDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BloqueDTO>> Create([FromBody] int utilisateurBloqueID)
        {
            int userId = await _currentUserService.GetUserIdOrThrow();
            bool exists = await _bloqueRepo.Exists(userId, utilisateurBloqueID);
            if (exists)
                return BadRequest("Cet utilisateur est déjà bloqué.");
            Abonnement abonnement = await _abonnementManager.FindAbonnement(userId, utilisateurBloqueID);
            if (abonnement != null)
            {
                await _abonnementManager.DeleteAsync(abonnement);
            }
            Bloque bloque = new Bloque()
            {
                UtilisateurBloqueurId = userId,
                UtilisateurBloqueId = utilisateurBloqueID
            };
            await _bloqueRepo.AddAsync(bloque);
            return Ok(_mapper.Map<BloqueDTO>(bloque));
        }
        /// <summary>
        /// Débloque un utilisateur précédemment bloqué.
        /// </summary>
        /// <param name="utilisateurBloqueId">L'identifiant de l'utilisateur à débloquer.</param>
        /// <returns>Aucun contenu en cas de succès.</returns>
        /// <response code="204">L'utilisateur a été débloqué avec succès.</response>
        /// <response code="404">Le blocage n'existe pas.</response>
        /// <response code="401">L'utilisateur n'est pas authentifié.</response>
        /// <response code="500">Erreur interne du serveur.</response>
        [HttpDelete("{utilisateurBloqueId}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
