using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.DTO;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ModerateurController : ControllerBase
{
    private readonly IModerationDashboardService  _moderationDashboardService;

    public ModerateurController(IModerationDashboardService moderationDashboardService)
    {
        _moderationDashboardService = moderationDashboardService;
    }

    [HttpGet("Statistics")]
    [Authorize(Roles = "Admin,Moderateur")]
    public async Task<ActionResult<DashBoardStatistics>> GetDashboardStatistics()
    {
        return await  _moderationDashboardService.GetDashboardStatistics();
    }

    [HttpGet("Activity")]
    [Authorize(Roles = "Admin,Moderateur")]
    public async Task<ActionResult<IEnumerable<ActivityDTO>>> GetActivity()
    {
        return await _moderationDashboardService.ListActivity();
    }
}