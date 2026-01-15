using API.Models.EntityFramework;
using Shared.DTO.Notification;

namespace API.Services;

public interface INotificationService
{
    Task CreateNotification(NotificationCreateDTO notification);
    Task DeleteAnnonceNotificationForUser(int annonceId);
    Task DeleteMessagesNotificationByConversationId(int conversationId, int userId);
}