using API.Models.EntityFramework;
using API.Services;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.DTO.SupportTicket;

namespace API.Controllers
{
    [ApiController]
    [Route("api/support")]
    public class SupportController : ControllerBase
    {
        private readonly ISupportService _supportService;
        private readonly ICurrentUserService _currentUser;

        public SupportController(
            ISupportService supportService,
            ICurrentUserService currentUser)
        {
            _supportService = supportService;
            _currentUser = currentUser;
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
            return Ok(await _supportService.GetOpenTicketsAsync());
        }

        [Authorize(Roles = "Admin, Moderateur")]
        [HttpPost("reply")]
        public async Task<IActionResult> Reply(SupportTicketReplyDTO dto)
        {
            int adminId = await _currentUser.GetUserIdOrThrow();
            await _supportService.ReplyAsync(adminId, dto);
            return Ok();
        }
    }
}
