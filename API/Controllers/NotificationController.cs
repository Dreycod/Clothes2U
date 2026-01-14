using Shared.DTO.Notification;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
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
    [HttpGet("user")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetUserNotifications()
    {
        int userId = await _currentUserService.GetUserIdOrThrow();
        var notifications = await _notificationManager.GetByUserId((int)userId);
        var notificationDtos = _mapper.Map<IEnumerable<NotificationDTO>>(notifications);
        await _notificationManager.MarkAsRead(userId);
        return Ok(notificationDtos);
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

    [HttpGet("id/{id}")]
    [ProducesResponseType(typeof(NotificationDTO),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<NotificationDTO>> GetById(int id)
    {
        var notification = await _notificationManager.GetByIdAsync(id);
        if (notification == null)
            return NotFound();
        NotificationDTO notificationDTO = _mapper.Map<NotificationDTO>(notification);
        return Ok(notificationDTO);
    }
}