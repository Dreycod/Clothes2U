namespace FrontBlazor.Services.Interfaces;

public interface ISignalRService
{
    event Action<int, int, string, DateTime>? OnMessageReceived;
    event Action<int, int, string>? OnUserTyping;
    event Action<int, int>? OnMessagesRead;
    
    bool IsConnected { get; }
    
    Task StartAsync();
    Task StopAsync();
    Task JoinConversation(int conversationId);
    Task LeaveConversation(int conversationId);
    Task SendMessage(int conversationId, int senderId, string message);
    Task NotifyTyping(int conversationId, int userId, string userName);
    Task MarkAsRead(int conversationId, int userId);
}
// namespace FrontBlazor.Services.Interfaces;
//
// public interface ISignalRService
// {
//     Task StartAsync();
//     Task StopAsync();
//     Task JoinConversation(int conversationId);
//     Task LeaveConversation(int conversationId);
//     Task SendMessage(int conversationId, int senderId, string message);
//     Task NotifyTyping(int conversationId, int userId, string userName);
//     Task MarkAsRead(int conversationId, int userId);
//
//     event Action<int, int, string, DateTime>? OnMessageReceived;
//     event Action<int, int, string>? OnUserTyping;
//     event Action<int, int>? OnMessagesRead;
//
//     bool IsConnected { get; }
// }