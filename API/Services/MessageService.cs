using API.Models.Repository.Interfaces;
using API.Services.Interfaces;

namespace API.Services;

public class MessageService : IMessageService
{
    
    private readonly IMessageRepository _messageRepository;
    private readonly INotificationHubService _notificationHubService;

    public MessageService(
        IMessageRepository messageRepository,
        INotificationHubService notificationHubService)
    {
        _messageRepository = messageRepository;
        _notificationHubService = notificationHubService;
    }
    public async Task SendMessageCount(int userId)
    {
        int unreadCount = await _messageRepository.GetMessageCountByUserId(userId);
        await _notificationHubService.UpdateUnreadMesssageCount(userId, unreadCount);
    }
}