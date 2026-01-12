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

namespace API.Controllers
{
    

    [Microsoft.AspNetCore.Components.Route("api/[controller]")]
    [ApiController]
    [Route("api/[controller]")]
    public class SupportController : ControllerBase
    {
        private readonly ISupportService _supportService;
        private readonly IDataRepository<SupportTicket, int> _ticketSupportRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public SupportController(
            ISupportService supportService,
            ICurrentUserService currentUser,
            IMapper mapper,
            IDataRepository<SupportTicket, int> ticketSupportRepository)
        {
            _supportService = supportService;
            _currentUser = currentUser;
            _ticketSupportRepository = ticketSupportRepository;
            _mapper = mapper;
        }

        // USER
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTicket(SupportTicketCreateDTO dto)
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
            int userId = await _currentUser.GetUserIdOrThrow();
            await _supportService.CreateTicketAsync(userId, dto);
            return Ok(APIResponse<SupportTicketCreateDTO>.SuccessResponse(dto));
        }

        // ADMIN
        [Authorize(Roles = "Admin, Moderateur")]
        [HttpGet]
        public async Task<IActionResult> GetTickets()
        {
            IEnumerable<SupportTicket> tickets = await _supportService.GetOpenTicketsAsync();
            IEnumerable<SupportTicketViewDTO> ticketsDTO = _mapper.Map<IEnumerable<SupportTicketViewDTO>>(tickets);
            return Ok(ticketsDTO);
        }

        [Authorize(Roles = "Admin, Moderateur")]
        [HttpPost("reply")]
        public async Task<IActionResult> Reply(SupportTicketReplyDTO dto)
        {
            int adminId = await _currentUser.GetUserIdOrThrow();
            await _supportService.ReplyAsync(adminId, dto);
            return Ok();
        }

        [HttpGet("id/{id}")]
        [Authorize(Roles = "Admin, Moderateur")]
        public async Task<IActionResult> GetTicket(int id)
        {
            SupportTicket ticket = await _ticketSupportRepository.GetByIdAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            SupportTicketDetailViewDTO dto = _mapper.Map<SupportTicketDetailViewDTO>(ticket);
            return Ok(dto);
        }
    }
}
