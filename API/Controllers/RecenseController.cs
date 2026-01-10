using Shared.DTO.Recense;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecenseController : ControllerBase
    {
        private readonly IRecenseRepository<Recense, int> _recenseManager;
        private readonly IMapper _mapper;

        public RecenseController(IRecenseRepository<Recense, int> recenseManager, IMapper mapper)
        {
            _recenseManager = recenseManager;
            _mapper = mapper;
        }

        // -------------------------
        // GET /id/{id}
        // -------------------------
        [HttpGet("id/{id}")]
        public async Task<ActionResult<RecenseDetailDTO>> GetById(int id)
        {
            var rec = await _recenseManager.GetByRecenseId(id);

            var r = rec.FirstOrDefault();
            if (r == null)
                return NotFound();

            return Ok(_mapper.Map<RecenseDetailDTO>(r));
        }

        // -------------------------
        // GET /TagsByAnnonce/{annonceId}
        // Liste des tags d'une annonce.
        // -------------------------
        [HttpGet("TagsByAnnonce/{annonceId}")]
        public async Task<ActionResult<IEnumerable<RecenseDetailDTO>>> GetTagsByAnnonce(int annonceId)
        {
            var rec = await _recenseManager.GetAllTagByAnnonceId(annonceId);
            return Ok(_mapper.Map<IEnumerable<RecenseDetailDTO>>(rec));
        }

        // -------------------------
        // GET /AnnoncesByTag/{tagId}
        // Liste des annonces par reliées à un tag
        // -------------------------
        [HttpGet("AnnoncesByTag/{tagId}")]
        public async Task<ActionResult<IEnumerable<RecenseDetailDTO>>> GetAnnoncesByTag(int tagId)
        {
            var rec = await _recenseManager.GetAllAnnonceByTagId(tagId);
            return Ok(_mapper.Map<IEnumerable<RecenseDetailDTO>>(rec));
        }

        // -------------------------
        // POST /Search
        // -------------------------
        [HttpPost("Search")]
        public async Task<ActionResult<IEnumerable<RecenseDetailDTO>>> Search([FromBody] RecenseSearchRequestDTO request)
        {
            var rec = await _recenseManager.SearchAsync(request);
            return Ok(_mapper.Map<IEnumerable<RecenseDetailDTO>>(rec));
        }

        // -------------------------
        // POST /
        // -------------------------
        [HttpPost]
        public async Task<ActionResult<RecenseDTO>> Add([FromBody] CreateRecenseDTO dto)
        {
            var entity = _mapper.Map<Recense>(dto);

            await _recenseManager.AddAsync(entity);

            var result = _mapper.Map<RecenseDTO>(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.RecenseId }, result);
        }

        // -------------------------
        // DELETE /id/{id}
        // -------------------------
        [HttpDelete("id/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var rec = await _recenseManager.GetByIdAsync(id);
            if (rec == null)
                return NotFound();

            await _recenseManager.DeleteAsync(rec);
            return NoContent();
        }
    }
}
