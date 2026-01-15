using System.Collections.ObjectModel;
using Shared.DTO.Moderation;
using Shared.DTO.Notification;
using FrontBlazor.Services.GenericService;


namespace FrontBlazor.Services.Interfaces;

public interface INotificationService 
{
    Task<ObservableCollection<NotificationDTO>> GetAllAsync();
    Task DeleteNotification(int notificationId);
    Task PostCommercialNotification(NotificationCommercialCreateDTO notification);
}