using FrontBlazor.Models;

namespace FrontBlazor.Services.Interfaces;

public interface IMessageNotificationService
{
    event Action<MessageDTO>? OnMessageReceived;
    event Action? OnUnreadCountChanged;
    int UnreadCount { get; }
    void NotifyMessageReceived(int conversationId, string senderName, string messageContent, int annonceId, string annonceTitle);
    void MarkConversationAsRead(int conversationId);
    void ClearNotification();
}