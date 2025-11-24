using API.DTO.Notification;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


[Microsoft.AspNetCore.Components.Route("api/[controller]")]
[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationRepository<Notification> _notificationManager;
    private readonly IMapper _mapper;

    public NotificationController(INotificationRepository<Notification> notificationManager, IMapper mapper)
    {
        _notificationManager = notificationManager;
        _mapper = mapper;
    }

    [HttpGet("byUserId/{userId}")]
    [ProducesResponseType(typeof(IEnumerable<NotificationDTO>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<IEnumerable<NotificationDTO>>> GetNotificationsByUserId(int userId)
    {
        IEnumerable<Notification> notifications = await _notificationManager.GetByUserId(userId);
        IEnumerable<NotificationDTO> notificationDtos = _mapper.Map<IEnumerable<NotificationDTO>>(notifications);
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
}