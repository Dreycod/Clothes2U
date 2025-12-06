using API.DTO.NoteUtilisateur;
using API.Models.EntityFramework;
using API.Models.Repository;
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
        private readonly IMapper _mapper;

        public NoteUtilisateurController(INoteUtilisateurRepository repo, IMapper mapper)
        {
            _noteUtilisateurManager = repo;
            _mapper = mapper;
        }

        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<NoteUtilisateurDTO>>> GetNotesByUserId(int userId)
        {
            var notes = await _noteUtilisateurManager.GetByUserIdAsync(userId);
            return Ok(_mapper.Map<IEnumerable<NoteUtilisateurDTO>>(notes));
        }

        [HttpGet("User/{userId}/Average")]
        public async Task<ActionResult<double>> GetMoyenne(int userId)
        {
            var avg = await _noteUtilisateurManager.GetMoyenneNoteAsync(userId);
            return Ok(avg);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NoteUtilisateurDetailDTO>> GetById(int id)
        {
            var note = await _noteUtilisateurManager.GetByIdAsync(id);
            if (note == null)
                return NotFound();

            return Ok(_mapper.Map<NoteUtilisateurDetailDTO>(note));
        }
        private int? GetConnectedUserId()
        {
            if (User?.Identity?.IsAuthenticated == true)
            {
                var userIdClaim = User.FindFirst("userId")?.Value;

                if (!string.IsNullOrEmpty(userIdClaim) && int.TryParse(userIdClaim, out int id))
                {
                    return id;
                }
            }
            return null;
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<NoteUtilisateurDTO>> AddNote(NoteUtilisateurCreateDTO dto)
        {
            int? userId = GetConnectedUserId();
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
