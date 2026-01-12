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
            }
            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = await _currentUserService.GetUserId();
            if (userId.HasValue)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"User_{userId.Value}");
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}