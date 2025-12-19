using System.Collections.ObjectModel;
using Shared.DTO.Moderation;
using Shared.DTO.Notification;
using FrontBlazor.Services.GenericIServices;


namespace FrontBlazor.Services.Interfaces;

public interface INotificationService 
{
    Task<ObservableCollection<NotificationDTO>> GetAllAsync();
    Task MarkAsRead();
    Task DeleteNotification(int notificationId);
    Task CreateNotificationAvertissement(CreateAvertissementRequestDTO request);
}