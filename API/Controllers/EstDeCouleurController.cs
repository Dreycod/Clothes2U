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

        // -------------------------
        // GET /
        // Liste de tous les EstDeCouleur
        // -------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EstDeCouleurDTO>>> GetAll()
        {
            var estDeCouleurs = await _estDeCouleurManager.GetAllWithDetailsAsync();
            return Ok(_mapper.Map<IEnumerable<EstDeCouleurDTO>>(estDeCouleurs));
        }

        // -------------------------
        // GET /id/{id}
        // -------------------------
        [HttpGet("id/{id}")]
        public async Task<ActionResult<EstDeCouleurDTO>> GetById(int id)
        {
            var estDeCouleur = await _estDeCouleurManager.GetByEstDeCouleurId(id);
            if (estDeCouleur == null)
                return NotFound();
            return Ok(_mapper.Map<EstDeCouleurDTO>(estDeCouleur));
        }

        // -------------------------
        // POST /
        // -------------------------
        [HttpPost]
        public async Task<ActionResult<EstDeCouleurDTO>> CreateEstDeCouleur([FromBody] CreateEstDeCouleurDTO request)
        {
            var estDeCouleurEntity = _mapper.Map<Est_De_Couleur>(request);
            await _estDeCouleurManager.AddAsync(estDeCouleurEntity);
            
            var estDeCouleurDto = _mapper.Map<EstDeCouleurDTO>(estDeCouleurEntity);
            return CreatedAtAction(nameof(GetById), new { id = estDeCouleurDto.EstDeCouleurId }, estDeCouleurDto);
        }

        // -------------------------
        // PUT /id/{id}
        // -------------------------
        [HttpPut("id/{id}")]
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

        // -------------------------
        // DELETE /id/{id}
        // -------------------------
        [HttpDelete("id/{id}")]
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