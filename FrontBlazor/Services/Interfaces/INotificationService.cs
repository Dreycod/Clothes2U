using System.Collections.ObjectModel;
using FrontBlazor.Models.Notification;
using FrontBlazor.Services.GenericIServices;


namespace FrontBlazor.Services.Interfaces;

public interface INotificationService 
{
    Task<ObservableCollection<Notification>> GetAllAsync(int utilisateurId);
    Task MarkAsRead();
    Task DeleteNotification(int notificationId);
}