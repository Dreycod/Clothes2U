using System.Collections.ObjectModel;
using FrontBlazor.Services.Interfaces;
using Shared.DTO.Conversation;
using Shared.DTO.Message;

namespace FrontBlazor.Services.WebService;

public class SignalRHandlerWebService : IDisposable
{
    private readonly ISignalRService _signalRService;
    private readonly IMessageService _messageService;
    
    // ✅ État local pour le contexte
    private ConversationDTO? _selectedConversation;
    private ObservableCollection<ConversationDTO> _conversations = new();
    private int? _selectedConversationId;
    private int _currentUserId;
    
    public event Action? OnStateChanged;
    public event Action? OnMessageReceivedUI;
    
    public bool IsTyping { get; private set; }
    public string TypingUserName { get; private set; } = "";
    private System.Threading.Timer? _typingDisplayTimer;
    
    public SignalRHandlerWebService(
        ISignalRService signalRService,
        IMessageService messageService)
    {
        _signalRService = signalRService;
        _messageService = messageService;
        
        // ✅ S'abonner avec les méthodes intermédiaires
        _signalRService.OnMessageReceived += OnMessageReceivedFromSignalR;
        _signalRService.OnUserTyping += OnUserTypingFromSignalR;
        _signalRService.OnMessagesRead += OnMessagesReadFromSignalR;
        _signalRService.OnProposalResponse += OnProposalResponseFromSignalR;
        _signalRService.OnPriceProposalReceived += OnPriceProposalReceivedFromSignalR;
    }
    
    /// <summary>
    /// ✅ IMPORTANT : Mettre à jour le contexte depuis le ViewModel
    /// </summary>
    public void SetContext(
        ConversationDTO? selectedConversation,
        ObservableCollection<ConversationDTO> conversations,
        int? selectedConversationId,
        int currentUserId)
    {
        _selectedConversation = selectedConversation;
        _conversations = conversations;
        _selectedConversationId = selectedConversationId;
        _currentUserId = currentUserId;
    }
    
    #region Méthodes intermédiaires (adaptateurs SignalR -> Handler)
    
    private async void OnMessageReceivedFromSignalR(
        int conversationId,
        int senderId,
        string message,
        List<int> photoIds,
        DateTime date)
    {
        await HandleMessageReceived(
            conversationId,
            senderId,
            message,
            photoIds,
            date);
    }
    
    private void OnUserTypingFromSignalR(
        int conversationId,
        int userId,
        string userName)
    {
        HandleUserTyping(conversationId, userId, userName);
    }
    
    private void OnMessagesReadFromSignalR(
        int conversationId,
        int userId)
    {
        HandleMessagesRead(conversationId, userId);
    }
    
    private void OnProposalResponseFromSignalR(
        int conversationId,
        int messageId,
        bool accepted)
    {
        HandleProposalResponse(conversationId, messageId, accepted);
    }
    
    private void OnPriceProposalReceivedFromSignalR(
        int conversationId,
        int messageId,
        int senderId,
        decimal proposedPrice,
        DateTime date)
    {
        HandlePriceProposalReceived(
            conversationId,
            messageId,
            senderId,
            proposedPrice,
            date);
    }
    
    #endregion
    
    #region Handlers (utilisant le contexte local)
    
    private async Task HandleMessageReceived(
        int conversationId,
        int senderId,
        string message,
        List<int> photoIds,
        DateTime date)
    {
        if (_selectedConversation != null && _selectedConversation.ConversationId == conversationId)
        {
            var exists = _selectedConversation.ListMessages?.Any(m =>
                m.SenderId == senderId &&
                m.Date.HasValue &&
                Math.Abs((m.Date.Value - date).TotalSeconds) < 2
            ) ?? false;

            if (!exists)
            {
                var newMessage = new MessageTextDTO()
                {
                    Content = message,
                    SenderId = senderId,
                    Date = date,
                    SentByCurrentUser = senderId == _currentUserId,
                    Photos = photoIds,
                    Lu = false
                };

                _selectedConversation.ListMessages?.Add(newMessage);

                var previewConv = _conversations.FirstOrDefault(c => c.ConversationId == conversationId);
                if (previewConv != null)
                {
                    previewConv.LastMessage = message;
                }
                
                if (senderId != _currentUserId)
                {
                    try
                    {
                        await _messageService.MaskAsRead(newMessage.MessageId.Value);
                        newMessage.Lu = true;
                        await _signalRService.MarkMessagesAsRead(conversationId, _currentUserId);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[SignalRHandler] ❌ Error auto-marking message as read: {ex.Message}");
                    }
                }
            }

            NotifyStateChanged();
            OnMessageReceivedUI?.Invoke();
        }
        else
        {
            var conv = _conversations.FirstOrDefault(c => c.ConversationId == conversationId);
            if (conv != null)
            {
                conv.LastMessage = message;
                conv.HasNewMessages = true;

                try
                {
                    _conversations.Remove(conv);
                    _conversations.Insert(0, conv);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SignalRHandler] Error moving conversation to top: {ex.Message}");
                }
            }

            NotifyStateChanged();
        }
    }
    
    private void HandleMessagesRead(int conversationId, int userId)
    {
        if (userId != _currentUserId)
        {
            ConversationDTO? targetConv = null;
        
            if (_selectedConversationId == conversationId && _selectedConversation != null)
            {
                targetConv = _selectedConversation;
            }
            else
            {
                targetConv = _conversations.FirstOrDefault(c => c.ConversationId == conversationId);
            }

            if (targetConv?.ListMessages != null)
            {
                var mySentMessages = targetConv.ListMessages
                    .Where(m => m.SenderId == _currentUserId && m.SentByCurrentUser == true && m.Lu == false)
                    .ToList();
            
                if (mySentMessages.Any())
                {
                    foreach (var m in mySentMessages)
                    {
                        m.Lu = true;
                    }
                    NotifyStateChanged();
                }
            }
        }
    }
    
    private void HandleUserTyping(int conversationId, int userId, string userName)
    {
        if (_selectedConversationId != conversationId || userId == _currentUserId)
        {
            return;
        }

        IsTyping = true;
        TypingUserName = userName;
        NotifyStateChanged();

        _typingDisplayTimer?.Dispose();
        _typingDisplayTimer = new System.Threading.Timer(_ =>
        {
            IsTyping = false;
            TypingUserName = "";
            NotifyStateChanged();
        }, null, 3000, Timeout.Infinite);
    }
    
    private void HandleProposalResponse(int conversationId, int messageId, bool accepted)
    {
        if (_selectedConversation?.ConversationId == conversationId)
        {
            var message = _selectedConversation.ListMessages?
                .OfType<MessageDemandeDTO>()
                .FirstOrDefault(m => m.MessageId == messageId);

            if (message != null)
            {
                message.EstAcceptee = accepted;
                message.EstRepondue = true;
                _selectedConversation.Prix = accepted ? message.PrixPropose : 0;
                NotifyStateChanged();
            }
        }
    }
    
    private void HandlePriceProposalReceived(
        int conversationId,
        int messageId,
        int senderId,
        decimal proposedPrice,
        DateTime date)
    {
        if (_selectedConversation?.ConversationId == conversationId)
        {
            var exists = _selectedConversation.ListMessages?.Any(m => m.MessageId == messageId) ?? false;

            if (!exists)
            {
                var newDemande = new MessageDemandeDTO
                {
                    MessageId = messageId,
                    ConversationId = conversationId,
                    SenderId = senderId,
                    Date = date,
                    PrixPropose = proposedPrice,
                    EstAcceptee = false,
                    EstRepondue = false,
                    SentByCurrentUser = senderId == _currentUserId
                };
            
                _selectedConversation.ListMessages?.Add(newDemande);
                NotifyStateChanged();
                OnMessageReceivedUI?.Invoke();
            }
        }
        else
        {
            var conv = _conversations.FirstOrDefault(c => c.ConversationId == conversationId);
            if (conv != null)
            {
                conv.LastMessage = $"Proposition: {proposedPrice} €";
                conv.HasNewMessages = true;

                try
                {
                    _conversations.Remove(conv);
                    _conversations.Insert(0, conv);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[SignalRHandler] Error moving conversation to top: {ex.Message}");
                }
            }

            NotifyStateChanged();
        }
    }
    
    #endregion
    
    private void NotifyStateChanged() => OnStateChanged?.Invoke();
    
    public void Dispose()
    {
        _signalRService.OnMessageReceived -= OnMessageReceivedFromSignalR;
        _signalRService.OnUserTyping -= OnUserTypingFromSignalR;
        _signalRService.OnMessagesRead -= OnMessagesReadFromSignalR;
        _signalRService.OnProposalResponse -= OnProposalResponseFromSignalR;
        _signalRService.OnPriceProposalReceived -= OnPriceProposalReceivedFromSignalR;
        _typingDisplayTimer?.Dispose();
    }
}