using Shared.DTO.Message;

namespace FrontBlazor.Services.Interfaces;

public interface ISignalRService
{
    // ✅ Événements SIMPLES pour le chat (pas d'objets complexes)
    event Action<int, int, string, List<int>, DateTime>? OnMessageReceived;
    event Action<int, int, string>? OnUserTyping;
    event Action<int, int>? OnMessagesRead;
    event Action<int, int, bool>? OnProposalResponse;
    event Action<int, int, int, decimal, DateTime>? OnPriceProposalReceived;
    
    // Événement pour les notifications
    event Action<int>? OnNotificationCountUpdated;
    
    // Propriétés de connexion
    bool IsConnected { get; }
    bool IsNotificationConnected { get; }
    
    // Méthodes pour le chat
    Task StartAsync();
    Task StopAsync();
    Task JoinConversation(int conversationId);
    Task LeaveConversation(int conversationId);
    Task SendMessage(int conversationId, int senderId, string message, List<int> photoIds);
    Task NotifyTyping(int conversationId, int userId, string userName);
    Task MarkMessagesAsRead(int conversationId, int userId);
    Task NotifyProposalResponse(int conversationId, int messageId, bool accepted);
    
    // Méthodes pour le hub de notifications
    Task StartNotificationHubAsync();
    Task StopNotificationHubAsync();
}