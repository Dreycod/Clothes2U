using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Couleur;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstDeCouleurController : ControllerBase
    {
        private readonly IEstDeCouleurRepository<Est_De_Couleur, int> _estDeCouleurManager;
        private readonly IMapper _mapper;

        public EstDeCouleurController(IEstDeCouleurRepository<Est_De_Couleur, int> estDeCouleurManager, IMapper mapper)
        {
            _estDeCouleurManager = estDeCouleurManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Récupère la liste de toutes les associations de couleurs.
        /// </summary>
        /// <returns>Une collection d'associations de couleurs avec leurs détails.</returns>
        /// <response code="200">Retourne la liste des associations de couleurs.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EstDeCouleurDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EstDeCouleurDTO>>> GetAll()
        {
            var estDeCouleurs = await _estDeCouleurManager.GetAllWithDetailsAsync();
            return Ok(_mapper.Map<IEnumerable<EstDeCouleurDTO>>(estDeCouleurs));
        }

        /// <summary>
        /// Récupère une association de couleur spécifique par son identifiant.
        /// </summary>
        /// <param name="id">L'identifiant de l'association de couleur.</param>
        /// <returns>Les détails de l'association de couleur.</returns>
        /// <response code="200">Retourne l'association de couleur demandée.</response>
        /// <response code="404">Association de couleur introuvable.</response>
        [HttpGet("id/{id}")]
        [ProducesResponseType(typeof(EstDeCouleurDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EstDeCouleurDTO>> GetById(int id)
        {
            var estDeCouleur = await _estDeCouleurManager.GetByEstDeCouleurId(id);
            if (estDeCouleur == null)
                return NotFound();
            return Ok(_mapper.Map<EstDeCouleurDTO>(estDeCouleur));
        }

        /// <summary>
        /// Crée une nouvelle association de couleur.
        /// </summary>
        /// <param name="request">Les données de l'association de couleur à créer.</param>
        /// <returns>L'association de couleur créée.</returns>
        /// <response code="201">Association de couleur créée avec succès.</response>
        /// <response code="400">Requête invalide - données manquantes ou incorrectes.</response>
        [HttpPost]
        [ProducesResponseType(typeof(EstDeCouleurDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<EstDeCouleurDTO>> CreateEstDeCouleur([FromBody] CreateEstDeCouleurDTO request)
        {
            var estDeCouleurEntity = _mapper.Map<Est_De_Couleur>(request);
            await _estDeCouleurManager.AddAsync(estDeCouleurEntity);
            
            var estDeCouleurDto = _mapper.Map<EstDeCouleurDTO>(estDeCouleurEntity);
            return CreatedAtAction(nameof(GetById), new { id = estDeCouleurDto.EstDeCouleurId }, estDeCouleurDto);
        }

        /// <summary>
        /// Met à jour une association de couleur existante.
        /// </summary>
        /// <param name="id">L'identifiant de l'association de couleur à mettre à jour.</param>
        /// <param name="dto">Les nouvelles données de l'association de couleur.</param>
        /// <returns>Aucun contenu en cas de succès.</returns>
        /// <response code="204">Association de couleur mise à jour avec succès.</response>
        /// <response code="400">Requête invalide - l'identifiant ne correspond pas.</response>
        /// <response code="404">Association de couleur introuvable.</response>
        [HttpPut("id/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateEstDeCouleur(int id, [FromBody] EstDeCouleurDTO dto)
        {
            if (id != dto.EstDeCouleurId)
                return BadRequest();
            var existingEntity = await _estDeCouleurManager.GetByEstDeCouleurId(id);
            if (existingEntity == null)
                return NotFound();
            _mapper.Map(dto, existingEntity);
            await _estDeCouleurManager.UpdateAsync(existingEntity);
            return NoContent();
        }

        /// <summary>
        /// Supprime une association de couleur.
        /// </summary>
        /// <param name="id">L'identifiant de l'association de couleur à supprimer.</param>
        /// <returns>Aucun contenu en cas de succès.</returns>
        /// <response code="204">Association de couleur supprimée avec succès.</response>
        /// <response code="404">Association de couleur introuvable.</response>
        [HttpDelete("id/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteEstDeCouleur(int id)
        {
            var existingEntity = await _estDeCouleurManager.GetByEstDeCouleurId(id);
            if (existingEntity == null)
                return NotFound();
            await _estDeCouleurManager.DeleteAsync(existingEntity);
            return NoContent();
        }
    }
}