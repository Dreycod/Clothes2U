using API.Controllers;
using API.Hubs;
using API.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace API.Services;

public class NotificationHubService : INotificationHubService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public NotificationHubService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task UpdateNotificationCount(int userId, int count)
    {
        try
        {
            await _hubContext.Clients
                .Group($"User_{userId}")
                .SendAsync("UpdateNotificationCount", count);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ [NotificationHubService] Error: {ex.Message}");
        }
    }

    public async Task UpdateUnreadMesssageCount(int userId, int count)
    {
        try
        {
            await _hubContext.Clients
                .Group($"User_{userId}")
                .SendAsync("UpdateMessageCount", count);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ [NotificationHubService] Error: {ex.Message}");
        }
    }
}