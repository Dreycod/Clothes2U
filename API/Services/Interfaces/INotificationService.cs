using API.Models.EntityFramework;
using Shared.DTO.Notification;

namespace API.Services;

public interface INotificationService
{
    Task CreateNotification(NotificationCreateDTO notification);
}