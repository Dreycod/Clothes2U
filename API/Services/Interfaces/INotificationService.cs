using API.Models.EntityFramework;
using Shared.DTO.Notification;

namespace API.Services;

public interface INotificationService
{
    Task CreateNotification(NotificationCreateDTO notification);
    Task CreateModificationAnnonceNotification(int annonceId);
    Task CreateNotificationAchat(int annonceId);
    Task CreateNouvelleAnnonceNotification(int annonceId);
    Task DeleteAnnonceNotificationForUser(int annonceId);
    Task DeleteMessagesNotificationByConversationId(int conversationId, int userId);
    Task CreateNotificationAvertissement(int userId, string messageAvertissement);
}