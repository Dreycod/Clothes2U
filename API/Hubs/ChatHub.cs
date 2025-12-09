using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace API.Hubs;

[Authorize] // Optionnel : si vous voulez sécuriser le hub
public class ChatHub : Hub
{
    public async Task SendMessageToConversation(int conversationId, string messageContent, int userId, DateTime date)
    {
        Console.WriteLine($"📨 SendMessageToConversation: ConvId={conversationId}, UserId={userId}");
        
        // Envoyer à tous les membres du groupe de cette conversation
        await Clients.Group($"conversation_{conversationId}")
            .SendAsync("ReceiveMessage", conversationId, messageContent, userId, date);
    }

    public async Task JoinConversation(int conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        Console.WriteLine($"✅ User {Context.ConnectionId} joined conversation {conversationId}");
    }

    public async Task LeaveConversation(int conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        Console.WriteLine($"👋 User {Context.ConnectionId} left conversation {conversationId}");
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"🟢 Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"🔴 Client disconnected: {Context.ConnectionId}");
        if (exception != null)
        {
            Console.WriteLine($"   Error: {exception.Message}");
        }
        await base.OnDisconnectedAsync(exception);
    }
}