using Shared.DTO.Tag;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using API.Services;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagController : ControllerBase
    {
        private readonly ITagRepository<Tag, int> _tagManager;
        private readonly IMapper _mapper;
        private readonly IMotInterditService _motInterditService;

        public TagController(ITagRepository<Tag, int> tagManager, IMapper mapper, IMotInterditService motInterditService)
        {
            _tagManager = tagManager;
            _mapper = mapper;
            _motInterditService = motInterditService;
        }

        // -------------------------
        // GET /
        // Liste de tous les tags
        // -------------------------
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TagDTO>>> GetAll()
        {
            var tags = await _tagManager.GetAllTagsAsync();
            return Ok(_mapper.Map<IEnumerable<TagDTO>>(tags));
        }

        // -------------------------
        // GET /id/{id}
        // -------------------------
        [HttpGet("id/{id}")]
        public async Task<ActionResult<TagDTO>> GetById(int id)
        {
            var tag = await _tagManager.GetByTagId(id);

            if (tag == null)
                return NotFound();

            return Ok(_mapper.Map<TagDTO>(tag));
        }

        // -------------------------
        // POST /Search
        // -------------------------
        [HttpPost("Search")]
        public async Task<ActionResult<IEnumerable<TagDTO>>> Search([FromBody] CreateTagDTO request)
        {
            var tags = await _tagManager.SearchAsync(request);
            return Ok(_mapper.Map<IEnumerable<TagDTO>>(tags));
        }

        // -------------------------
        // POST /
        // -------------------------
        [HttpPost]
        public async Task<ActionResult<TagDTO>> Add([FromBody] CreateTagDTO dto)
        {
            var isForbidden = await _motInterditService.ContientMotInterdit(dto.Libelle);
            if (isForbidden)
            {
                return BadRequest("Mot Interdit");
            }
            var entity = _mapper.Map<Tag>(dto);

            await _tagManager.AddAsync(entity);

            var result = _mapper.Map<TagDTO>(entity);
            return CreatedAtAction(nameof(GetById), new { id = entity.TagId }, result);
        }

        // -------------------------
        // PUT /id/{id}
        // -------------------------
        [HttpPut("id/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] TagDTO dto)
        {
            var tag = await _tagManager.GetByIdAsync(id);
            if (tag == null)
                return NotFound();

            _mapper.Map(dto, tag);
            await _tagManager.UpdateAsync(tag);

            return NoContent();
        }

        // -------------------------
        // DELETE /id/{id}
        // -------------------------
        [HttpDelete("id/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var tag = await _tagManager.GetByIdAsync(id);
            if (tag == null)
                return NotFound();

            await _tagManager.DeleteAsync(tag);
            return NoContent();
        }
    }
}