using Shared.DTO.NoteUtilisateur;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Shared.DTO;
using Shared.DTO.Conversation;
using Shared;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NoteUtilisateurController : ControllerBase
    {
        private readonly INoteUtilisateurRepository _noteUtilisateurManager;
        private readonly IOrderRepository _orderManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly IConversationRepository<Conversation, int> _conversationManager;
        private readonly IMapper _mapper;

        public NoteUtilisateurController(INoteUtilisateurRepository noteUtilisateurManager, IOrderRepository orderManager, IConversationRepository<Conversation, int> conversationManager, ICurrentUserService currentUserService, IMapper mapper)
        {
            _noteUtilisateurManager = noteUtilisateurManager;
            _orderManager = orderManager;
            _currentUserService = currentUserService;
            _conversationManager = conversationManager;
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
            int userId = await _currentUserService.GetUserIdOrThrow();
            Commande order = await _orderManager.GetOrderWithDetailsAsync(dto.CibleId);
            Conversation conversation = await _conversationManager.GetByIdAsync(order.ConversationId);
            int? id = await _conversationManager.GetOtherUser(userId, conversation);
            if(id == null)
            {
                return NotFound();
            }

            var note = await _noteUtilisateurManager.GetNoteByUserIdAndOtherUserId((int)userId, (int)id);
            if (note != null)
            {
                return BadRequest(APIResponse<object>.ErrorResponse("Vous avez déjà laissé une note pour cet utilisateur."));
            }

            dto.CibleId = (int)id;
            var noteUtilisateur = _mapper.Map<NoteUtilisateur>(dto);
            noteUtilisateur.AuteurId = (int)userId;
            noteUtilisateur.Statut = true;
            await _noteUtilisateurManager.AddAsync(noteUtilisateur);

            return Ok(APIResponse<object>.SuccessResponse(null));
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
