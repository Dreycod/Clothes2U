using API.DTO.NoteUtilisateur;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteUtilisateurController : ControllerBase
    {
        private readonly INoteUtilisateurRepository _noteUtilisateurManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public NoteUtilisateurController(INoteUtilisateurRepository noteUtilisateurRepository, ICurrentUserService currentUserService, IMapper mapper)
        {
            _noteUtilisateurManager = noteUtilisateurRepository;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        [HttpGet("User/{userId}")]
        [ProducesResponseType(typeof(IEnumerable<NoteUtilisateurDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<NoteUtilisateurDTO>>> GetNotesByUserId(
            int userId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            if (page <= 0 || pageSize <= 0)
            {
                return BadRequest("Page et pageSize doivent être supérieurs à 0");
            }
    
            var notes = await _noteUtilisateurManager.GetByUserIdAsync(userId, page, pageSize);
    
            return Ok(_mapper.Map<IEnumerable<NoteUtilisateurDTO>>(notes));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NoteUtilisateurDetailDTO>> GetById(int id)
        {
            var note = await _noteUtilisateurManager.GetByIdAsync(id);
            if (note == null)
                return NotFound();

            return Ok(_mapper.Map<NoteUtilisateurDetailDTO>(note));
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<NoteUtilisateurDTO>> AddNote(NoteUtilisateurCreateDTO dto)
        {
            int? userId = await _currentUserService.GetUserId();
            if (userId == null)
            {
                return Unauthorized();
            }
            var entity = _mapper.Map<NoteUtilisateur>(dto);
            entity.AuteurId = (int)userId;
            await _noteUtilisateurManager.AddAsync(entity);

            return CreatedAtAction(nameof(GetById), new { id = entity.NoteUtilisateurId },
                _mapper.Map<NoteUtilisateurDTO>(entity));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var note = await _noteUtilisateurManager.GetByIdAsync(id);
            if (note == null)
                return NotFound();

            await _noteUtilisateurManager.DeleteAsync(note);
            return NoContent();
        }
    }
}
