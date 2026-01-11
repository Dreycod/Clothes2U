namespace API.Services.Interfaces;
public interface INotificationHubService
{
    Task UpdateNotificationCount(int userId, int count);
    Task UpdateUnreadMesssageCount(int userId, int count);
}