using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Interfaces;
using API.Services;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.DTO.SupportTicket;
using Shared.Enums;

namespace API.Controllers
{
    
    [Microsoft.AspNetCore.Components.Route("api/[controller]")]
    [ApiController]
    [Route("api/[controller]")]
    public class SupportController : ControllerBase
    {
        private readonly ISupportService _supportService;
        private readonly ITicketRepository _ticketManager;
        private readonly IMapper _mapper;

        public SupportController(
            ISupportService supportService,
            ITicketRepository ticketManager,
            IMapper mapper)
        {
            _supportService = supportService;
            _ticketManager = ticketManager;
            _mapper = mapper;
        }

        // USER
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTicket([FromBody]SupportTicketCreateDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(dto.Subject) || string.IsNullOrWhiteSpace(dto.Message))
            {
                return BadRequest(APIResponse<object>.ErrorResponse(
                    "Le sujet et le message sont obligatoires"
                ));
            }
            await _supportService.CreateTicket(dto);
            return Ok(APIResponse<SupportTicketCreateDTO>.SuccessResponse(dto));
        }

        // ADMIN
        [Authorize(Roles = "Admin, Moderateur")]
        [HttpGet]
        public async Task<IActionResult> GetTickets()
        {
            IEnumerable<Ticket> tickets = await _ticketManager.GetOpenTicketsAsync();
            IEnumerable<SupportTicketViewDTO> ticketsDTO = _mapper.Map<IEnumerable<SupportTicketViewDTO>>(tickets);
            return Ok(ticketsDTO);
        }
        [Authorize(Roles = "Admin, Moderateur")]
        [HttpGet("Pending")]
        public async Task<IActionResult> GetPendingTickets()
        {
            IEnumerable<Ticket> tickets = await _ticketManager.GetPendingTicketsAsync();
            IEnumerable<SupportTicketViewDTO> ticketsDTO = _mapper.Map<IEnumerable<SupportTicketViewDTO>>(tickets);
            return Ok(ticketsDTO);
        }
        
        [Authorize(Roles = "Admin, Moderateur")]
        [HttpPost("reply")]
        public async Task<IActionResult> Reply(SupportTicketReplyDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(dto.Message))
            {
                return BadRequest("Il est obligatoire d'entrer une réponse.");
            }
            try
            {
                await _supportService.Reply(dto);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [Authorize(Roles = "Admin, Moderateur")]
        [HttpPut("closeTicket/{id}")]
        public async Task<IActionResult> CloseTicket(int id)
        {
            Ticket ticket = await _ticketManager.GetByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ticket.Status = (int)StatusTicketEnum.CLOSED;
            await _ticketManager.UpdateAsync(ticket);
            return NoContent();
        }
        [HttpGet("id/{id}")]
        [Authorize(Roles = "Admin, Moderateur")]
        public async Task<IActionResult> GetTicket(int id)
        {
            Ticket ticket = await _ticketManager.GetByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            TicketDetailViewDTO dto = _mapper.Map<TicketDetailViewDTO>(ticket);
            return Ok(dto);
        }
    }
}
