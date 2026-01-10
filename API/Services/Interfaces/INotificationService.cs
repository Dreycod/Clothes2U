using API.Models.EntityFramework;
using Shared.DTO.Notification;

namespace API.Services;

public interface INotificationService
{
    Task CreateNotification(NotificationCreateDTO notification);
    Task CreateModificationAnnonceNotification(int annonceId);
    Task CreateNouvelleAnnonceNotification(int annonceId);
}