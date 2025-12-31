using Shared.DTO.Message;

namespace FrontBlazor.Services.Interfaces;

public interface ISignalRService
{
    event Action<int, int, string, List<int>, DateTime>? OnMessageReceived;
    event Action<int, int, string>? OnUserTyping;
    event Action<int, int>? OnMessagesRead;
    
    bool IsConnected { get; }
    
    Task StartAsync();
    Task StopAsync();
    Task JoinConversation(int conversationId);
    Task LeaveConversation(int conversationId);
    Task SendMessage(int conversationId, int senderId, string message, List<int> photoIds);
    Task NotifyTyping(int conversationId, int userId, string userName);
    Task MarkMessagesAsRead(int conversationId, int userId);
    // Task PostMessageDemande(MessageDemandePostDTO message);
    // Task AcceptPriceProposal(int messageId);
    // Task DeclinePriceProposal(int messageId);
    Task NotifyProposalResponse(int conversationId, int messageId, bool accepted);
    event Action<int, int, bool>? OnProposalResponse;
}
