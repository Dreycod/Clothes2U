using API.Models.EntityFramework;
using API.Services;
using API.Services.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            int userId = await _currentUser.GetUserIdOrThrow();
            await _supportService.CreateTicketAsync(userId, dto);
            return Ok();
        }

        // ADMIN
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetTickets()
        {
            return Ok(await _supportService.GetOpenTicketsAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("reply")]
        public async Task<IActionResult> Reply(SupportTicketReplyDTO dto)
        {
            int adminId = await _currentUser.GetUserIdOrThrow();
            await _supportService.ReplyAsync(adminId, dto);
            return Ok();
        }
    }
}
