using API.Services;
using Microsoft.AspNetCore.SignalR;

namespace API.Hubs
{
    public class NotificationHub : Hub
    {
        private readonly ICurrentUserService _currentUserService;

        public NotificationHub(ICurrentUserService currentUserService)
        {
            _currentUserService = currentUserService;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = await _currentUserService.GetUserId();
            if (userId.HasValue)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"User_{userId.Value}");
                Console.WriteLine($"✅ [NotificationHub] User {userId.Value} connected and added to group");
            }
            else
            {
                Console.WriteLine($"⚠️ [NotificationHub] Anonymous connection - userId not found");
                // On autorise la connexion même sans userId
                // Le service n'enverra des notifications qu'aux users authentifiés de toute façon
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = await _currentUserService.GetUserId();
            if (userId.HasValue)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId.Value}");
                Console.WriteLine($"🛑 [NotificationHub] User {userId.Value} disconnected");
            }
            
            Console.WriteLine("========================================");
            await base.OnDisconnectedAsync(exception);
        }
    }
}