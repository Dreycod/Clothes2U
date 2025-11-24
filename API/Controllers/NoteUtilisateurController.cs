using API.DTO.NoteUtilisateur;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteUtilisateurController : ControllerBase
    {
        private readonly INoteUtilisateurRepository _repo;
        private readonly IMapper _mapper;

        public NoteUtilisateurController(INoteUtilisateurRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<NoteUtilisateurDTO>>> GetNotesByUserId(int userId)
        {
            var notes = await _repo.GetByUserIdAsync(userId);
            return Ok(_mapper.Map<IEnumerable<NoteUtilisateurDTO>>(notes));
        }

        [HttpGet("User/{userId}/Average")]
        public async Task<ActionResult<double>> GetMoyenne(int userId)
        {
            var avg = await _repo.GetMoyenneNoteAsync(userId);
            return Ok(avg);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<NoteUtilisateurDetailDTO>> GetById(int id)
        {
            var note = await _repo.GetByIdAsync(id);
            if (note == null)
                return NotFound();

            return Ok(_mapper.Map<NoteUtilisateurDetailDTO>(note));
        }

        [HttpPost]
        public async Task<ActionResult<NoteUtilisateurDTO>> AddNote(NoteUtilisateurCreateDTO dto)
        {
            var entity = _mapper.Map<NoteUtilisateur>(dto);
            await _repo.AddAsync(entity);

            return CreatedAtAction(nameof(GetById), new { id = entity.NoteUtilisateurId },
                _mapper.Map<NoteUtilisateurDTO>(entity));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            var note = await _repo.GetByIdAsync(id);
            if (note == null)
                return NotFound();

            await _repo.DeleteAsync(note);
            return NoContent();
        }
    }
}
