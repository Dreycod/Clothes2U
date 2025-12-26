using Microsoft.AspNetCore.SignalR;

namespace API.Hubs;

public class ChatHub : Hub
{
    // Cette méthode n'est plus utilisée directement - c'est le controller qui broadcast
    public async Task SendMessage(int conversationId, int senderId, string message, List<int> photoIds)
    {
        var date = DateTime.UtcNow;

        await Clients.Group($"conversation_{conversationId}")
            .SendAsync("ReceiveMessage", conversationId, senderId, message, photoIds, date);
    }

    public async Task JoinConversation(int conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        Console.WriteLine($"[Hub] ✅ {Context.ConnectionId} joined conversation_{conversationId}");
    }

    public async Task LeaveConversation(int conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
        Console.WriteLine($"[Hub] 👋 {Context.ConnectionId} left conversation_{conversationId}");
    }

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"[Hub] 🟢 Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"[Hub] 🔴 Client disconnected: {Context.ConnectionId}");
        if (exception != null)
        {
            Console.WriteLine($"[Hub]    Error: {exception.Message}");
        }
        await base.OnDisconnectedAsync(exception);
    }
    
    public async Task MarkMessagesAsRead(int conversationId, int userId)
    {
        var groupName = $"conversation_{conversationId}";
        Console.WriteLine($"[Hub] 📖 User {userId} marked messages as read in conversation {conversationId}");
        
        // Notifier tous les membres du groupe
        await Clients.Group(groupName).SendAsync("MessagesRead", conversationId, userId);
        
        Console.WriteLine($"[Hub] ✅ Broadcasted MessagesRead to group {groupName}");
    }
    
    public async Task NotifyTyping(int conversationId, int userId, string userName)
    {
        var groupName = $"conversation_{conversationId}";
        Console.WriteLine($"[Hub] ⌨️ User {userId} ({userName}) typing in conv {conversationId}");
    
        await Clients.Group(groupName).SendAsync("UserTyping", conversationId, userId, userName);
    
        Console.WriteLine($"[Hub] ✅ Broadcasted typing to group {groupName}");
    }
}