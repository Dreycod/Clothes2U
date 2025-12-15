using System.Collections.ObjectModel;
using FrontBlazor.Models.Moderation;
using FrontBlazor.Models.Notification;
using FrontBlazor.Services.GenericIServices;


namespace FrontBlazor.Services.Interfaces;

public interface INotificationService 
{
    Task<ObservableCollection<Notification>> GetAllAsync();
    Task MarkAsRead();
    Task DeleteNotification(int notificationId);
    Task CreateNotificationAvertissement(CreateAvertissementRequest request);
}