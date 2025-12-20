using FrontBlazor.Models;
using FrontBlazor.Services.Interfaces;

namespace FrontBlazor.Services;

public class MessageNotificationWebService : IMessageNotificationService
{
    public event Action<MessageDTO>? OnMessageReceived;
    public event Action? OnUnreadCountChanged;
    private readonly List<int> _unreadConversations = new();
    public int UnreadCount => _unreadConversations.Count;

    public void NotifyMessageReceived(int conversationId, string senderName, string messageContent, int annonceId, string annonceTitle)
    {
        
        // Ajouter la conversation aux non lues si pas déjà présente
        if (!_unreadConversations.Contains(conversationId))
        {
            _unreadConversations.Add(conversationId);
            OnUnreadCountChanged?.Invoke();
        }

        // Créer la notification
        var notification = new MessageDTO
        {
            ConversationId = conversationId,
            SenderName = senderName,
            Content = messageContent.Length > 50 
                ? messageContent.Substring(0, 50) + "..." 
                : messageContent,
            Date = DateTime.UtcNow
        };

        Console.WriteLine($"[MessageNotificationService] 🔔 New message notification from {senderName}");
        OnMessageReceived?.Invoke(notification);
    }

    public void MarkConversationAsRead(int conversationId)
    {
        if (_unreadConversations.Contains(conversationId))
        {
            _unreadConversations.Remove(conversationId);
            OnUnreadCountChanged?.Invoke();
            Console.WriteLine($"[MessageNotificationService] ✅ Conversation {conversationId} marked as read. Remaining unread: {UnreadCount}");
        }
    }

    public void ClearNotification()
    {
        // Méthode pour fermer une notification (si besoin)
    }
}