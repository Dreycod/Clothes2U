using API.DTO.Notification;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;



public class CreateAvertissementRequest
{
    public string MessageAvertissement { get; set; }
    public int UtilisateurId { get; set; }
}



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
        int? userId = await _currentUserService.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }
        int count = await _notificationManager.GetNotificationsUnreadCountByUserId((int)userId);
        return count;
    }
    [HttpGet("user")]
    public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetUserNotifications()
    {
        int? userId = await _currentUserService.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }
        var notifications = await _notificationManager.GetByUserId((int)userId);
        var notificationDtos = _mapper.Map<IEnumerable<NotificationDTO>>(notifications);
    
        return Ok(notificationDtos);
    }
    [HttpPut("markAsRead")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> MarkAsRead()
    {
        int? userId = await _currentUserService.GetUserId();
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
    [HttpPost("avertissement")]
    [Authorize]
    public async Task<ActionResult<NotificationAvertissementDTO>> CreateNotificationAvertissement(
        [FromBody] CreateAvertissementRequest avertissementRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        Notification notification = new Notification()
        {
            DateCreation = DateTime.UtcNow,
            EstLu = false,
            NotificationTypeId = 3,
            UtilisateurId = avertissementRequest.UtilisateurId,
        };
        await _notificationManager.AddAsync(notification);
        NotificationAvertissement notificationAvertissement = new NotificationAvertissement()
        {
            NotificationId = notification.NotificationId,
            MessageAvertissement = avertissementRequest.MessageAvertissement,
        };
        await _notificationManager.CreateNotificationAvertissement(notificationAvertissement);
        return CreatedAtAction(nameof(GetById), new { id = notification.NotificationId }, notificationAvertissement);
    }
}