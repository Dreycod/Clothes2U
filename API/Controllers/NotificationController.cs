using API.DTO.Notification;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationRepository _notificationManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMapper _mapper;

    public NotificationController(INotificationRepository notificationManager, IMapper mapper, ICurrentUserService currentUserService)
    {
        _notificationManager = notificationManager;
        _currentUserService = currentUserService;
        _mapper = mapper;
    }
    [HttpGet("notificationCount")]
    [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<int>> GetNotificationsUnreadCountByUser()
    {
        int? userId = _currentUserService.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }
        int count = await _notificationManager.GetNotificationsUnreadCountByUserId((int)userId);
        return count;
    }
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetUserNotifications(int userId)
    {
        var notifications = await _notificationManager.GetByUserId(userId);
        var notificationDtos = _mapper.Map<IEnumerable<NotificationDTO>>(notifications);
    
        return Ok(notificationDtos);
    }
    [HttpPut("markAsRead")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MarkAsRead()
    {
        int? userId = _currentUserService.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }
        await _notificationManager.MarkAsRead((int)userId);
        return NoContent();
    }
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteNotification(int id)
    {
        Notification? notificationToDelete = await _notificationManager.GetByIdAsync(id);
        if (notificationToDelete == null)
        {
            return NotFound();
        }
        await _notificationManager.DeleteAsync(notificationToDelete);
        return NoContent();
    }
}