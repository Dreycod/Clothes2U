using API.DTO.Visualisation;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VisualisationController : ControllerBase
    {
        private readonly IVisualisationRepository<Visualisation, int> _visualisationManager;
        private readonly IMapper _mapper;

        public VisualisationController(IVisualisationRepository<Visualisation, int> visualisationManager, IMapper mapper)
        {
            _visualisationManager = visualisationManager;
            _mapper = mapper;
        }

        /// <summary>
        /// Retourne la liste des annonces visualisé par un utilisateur.
        /// </summary>
        /// <param name="id">L'id de l'utilisateur concerné</param>
        /// <returns></returns>
        [HttpGet("user/{id}")]
        public async Task<ActionResult<IEnumerable<VisualisationDetailDTO>>> GetAllAnnonceVisualiseByUserId(int id)
        {
            var result = await _visualisationManager.GetByUtilisateurId(id);
            return Ok(_mapper.Map<IEnumerable<VisualisationDetailDTO>>(result));

        }

        /// <summary>
        /// Retourne la liste des utilisateurs qui ont visualisé une annonce en particulier.
        /// </summary>
        /// <param name="id">L'id de l'annonce concernée.</param>
        /// <returns></returns>
        [HttpGet("annonce/{id}")]
        public async Task<ActionResult<IEnumerable<VisualisationDetailDTO>>> GetAllUserVisualiseByAnnonceId(int id)
        {
            var result = await _visualisationManager.GetByAnnonceId(id);
            return Ok(_mapper.Map<IEnumerable<VisualisationDetailDTO>>(result));

        }

        /// <summary>
        /// Crée une nouvelle ligne dans la base de donnée après qu'un utilisateur ait passé 5 secondes sur une page
        /// fais aussi attention à ne pas créer de doublon de la même instance.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> CreateVisualisation([FromBody] VisualisationCreateDTO dto)
        {
            TimeSpan delay = TimeSpan.FromSeconds(5);

            bool exists = await _visualisationManager.HasRecentView(dto.UtilisateurId, dto.AnnonceId, delay);

            if (exists)
                return NoContent(); // Pas de doublon

            var entity = _mapper.Map<Visualisation>(dto);

            await _visualisationManager.AddAsync(entity);

            return Ok(_mapper.Map<VisualisationDTO>(entity));
        }

        /// <summary>
        /// Retourne le nombre de vues d'une annonce.
        /// </summary>
        /// <param name="annonceId"></param>
        /// <returns></returns>
        [HttpGet("CountByAnnonce/{annonceId}")]
        [ProducesResponseType(typeof(TopAnnonceStatsDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<TopAnnonceStatsDTO>> GetViewCountByAnnonce(int annonceId)
        {
            int count = await _visualisationManager.CountViewsByAnnonce(annonceId);

            var dto = new TopAnnonceStatsDTO
            {
                AnnonceId = annonceId,
                NombreVues = count
            };

            return Ok(dto);
        }

        /// <summary>
        /// Retourne les x (x = limit) annonces les plus vues.
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        [HttpGet("Top/{limit}")]
        [ProducesResponseType(typeof(IEnumerable<TopAnnonceStatsDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TopAnnonceStatsDTO>>> GetTopAnnonces(int limit)
        {
            var data = await _visualisationManager.GetTopAnnonces(limit);

            var dto = data.Select(x => new TopAnnonceStatsDTO
            {
                AnnonceId = x.AnnonceId,
                NombreVues = x.NombreVues
            });

            return Ok(dto);
        }



    }
}
