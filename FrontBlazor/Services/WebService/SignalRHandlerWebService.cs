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
        _signalRService.OnPaymentReceived += OnPaymentReceivedFromSignalR;
        _signalRService.OnColisEnvoyeReceived += OnColisEnvoyeReceivedFromSignalR;
        _signalRService.OnColisRecuReceived += OnColisRecuReceivedFromSignalR;
        _signalRService.OnPaymentCancelled += OnPaymentCancelledFromSignalR;
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
        DateTime date,
        int messageId)
    {
        await HandleMessageReceived(
            conversationId,
            senderId,
            message,
            photoIds,
            date,
            messageId);
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
    
    private void OnCancelPaymentRecievedFromSignalR(int conversationId, int messageId, int senderId, DateTime date, bool accepted)
    {
        HandleCancelPaymentReceived(conversationId, messageId, senderId, date, accepted);
    }
    
    #endregion
    
    #region Handlers (utilisant le contexte local)

    private async void HandleCancelPaymentReceived(int conversationId, int messageId, int senderId, DateTime date,
        bool accepted)
    {
        if (_selectedConversation?.ConversationId == conversationId)
        {
            var exists = _selectedConversation.ListMessages?.Any(m =>
                m.SenderId == senderId &&
                m.MessageId == messageId
            ) ?? false;

            if (!exists)
            {
                
            }
        }
    }
    
    
    // Dans SignalRHandlerWebService.cs

    private async Task HandleMessageReceived(
        int conversationId,
        int senderId,
        string message,
        List<int> photoIds,
        DateTime date,
        int? messageId)
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
                    MessageId = messageId,
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
                        await _messageService.MaskAsRead(messageId.Value);
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
                _selectedConversation.Prix = accepted ? message.PrixPropose : _selectedConversation.Prix;
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
    
    #region Nouveaux handlers SignalR
private void OnPaymentReceivedFromSignalR(
        int conversationId,
        int messageId,
        int messagePayeeId,
        int senderId,
        DateTime date)
    {
        HandlePaymentReceived(conversationId, messageId, messagePayeeId, senderId, date);
    }

    private void OnColisEnvoyeReceivedFromSignalR(
        int conversationId,
        int messageId,
        int messageEnvoieId,
        int senderId,
        int photoId,
        DateTime date,
        int messagePayeeId)
    {
        HandleColisEnvoyeReceived(conversationId, messageId, messageEnvoieId, senderId, photoId, date, messagePayeeId);
    }

    private void OnColisRecuReceivedFromSignalR(
        int conversationId,
        int messageId,
        int messageRecuId,
        int senderId,
        bool estConforme,
        int? photoId,
        string? description,
        DateTime date)
    {
        HandleColisRecuReceived(conversationId, messageId, messageRecuId, senderId, estConforme, photoId, description, date);
    }

    #endregion
    
    private void OnPaymentCancelledFromSignalR(
        int conversationId,
        int messagePayeeId,
        int userId)
    {
        HandlePaymentCancelled(conversationId, messagePayeeId, userId);
    }
    
    private void HandlePaymentCancelled(
        int conversationId,
        int messagePayeeId,
        int userId)
    {
        if (_selectedConversation?.ConversationId == conversationId)
        {
            // Trouver le message de paiement et le marquer comme annulé
            var messagePayee = _selectedConversation.ListMessages?
                .OfType<MessageEstPayeeDTO>()
                .FirstOrDefault(m => m.MessageId == messagePayeeId);
            
            _selectedConversation.AnnonceStatut = "En Ligne";
            _selectedConversation.StatusConversation = "En négociation";

            if (messagePayee != null)
            {
                messagePayee.EstAnnule = true;
                Console.WriteLine($"[SignalRHandler] ✅ Payment {messagePayeeId} marked as cancelled");
                NotifyStateChanged();
            }
            else
            {
                Console.WriteLine($"[SignalRHandler] ⚠️ Payment message {messagePayeeId} not found in conversation");
            }
        }
        else
        {
            // Si on n'est pas dans la conversation, mettre à jour le dernier message
            var conv = _conversations.FirstOrDefault(c => c.ConversationId == conversationId);
            if (conv != null)
            {
                conv.StatusConversation = "En négociation";
                conv.LastMessage = "Paiement annulé";
                conv.HasNewMessages = true;
                conv.AnnonceStatut = "En Ligne";

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

    #region Handlers internes

    private void HandlePaymentReceived(
        int conversationId,
        int messageId,
        int messagePayeeId,
        int senderId,
        DateTime date)
    {
        if (_selectedConversation?.ConversationId == conversationId)
        {
            var exists = _selectedConversation.ListMessages?.Any(m => m.MessageId == messageId) ?? false;

            if (!exists)
            {
                var newPayment = new MessageEstPayeeDTO
                {
                    MessageEstPayeeId = messagePayeeId,
                    MessageId = messageId,
                    ConversationId = conversationId,
                    SenderId = senderId,
                    Date = date,
                    SentByCurrentUser = senderId == _currentUserId,
                    EstEnvoye = false
                };

                _selectedConversation.StatusConversation = "Acceptation";
            
                _selectedConversation.ListMessages?.Add(newPayment);
                NotifyStateChanged();
                OnMessageReceivedUI?.Invoke();
            }
        }
        else
        {
            var conv = _conversations.FirstOrDefault(c => c.ConversationId == conversationId);
            if (conv != null)
            {
                conv.LastMessage = "Paiement effectué";
                conv.HasNewMessages = true;
                conv.StatusConversation = "Acceptation";

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

    private void HandleColisEnvoyeReceived(
        int conversationId,
        int messageId,
        int messageEnvoieId,
        int senderId,
        int photoId,
        DateTime date,
        int messagePayeeId)
    {
        if (_selectedConversation?.ConversationId == conversationId)
        {
            var exists = _selectedConversation.ListMessages?.Any(m => m.MessageId == messageId) ?? false;

            if (!exists)
            {
                var newColis = new MessageEnvoieColisDTO
                {
                    MessageEnvoieColisId = messageEnvoieId,
                    MessageId = messageId,
                    ConversationId = conversationId,
                    SenderId = senderId,
                    Date = date,
                    SentByCurrentUser = senderId == _currentUserId,
                    PhotoId = photoId,
                    MessageEstPayeeId = messagePayeeId
                };
                
                _selectedConversation.StatusConversation = "Terminée";
            
                _selectedConversation.ListMessages?.Add(newColis);
                
                // Mettre à jour le message payee correspondant
                var messagePayee = _selectedConversation.ListMessages?
                    .OfType<MessageEstPayeeDTO>()
                    .LastOrDefault(m => m.MessageEstPayeeId == messagePayeeId);
                
                if (messagePayee != null)
                {
                    messagePayee.EstEnvoye = true;
                }
                
                NotifyStateChanged();
                OnMessageReceivedUI?.Invoke();
            }
        }
        else
        {
            var conv = _conversations.FirstOrDefault(c => c.ConversationId == conversationId);
            if (conv != null)
            {
                conv.LastMessage = "Colis envoyé";
                conv.HasNewMessages = true;
                conv.StatusConversation = "Terminée";

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

    private void HandleColisRecuReceived(
        int conversationId,
        int messageId,
        int messageRecuId,
        int senderId,
        bool estConforme,
        int? photoId,
        string? description,
        DateTime date)
    {
        if (_selectedConversation?.ConversationId == conversationId)
        {
            var exists = _selectedConversation.ListMessages?.Any(m => m.MessageId == messageId) ?? false;

            if (!exists)
            {
                var newRecu = new MessageEstRecuDTO
                {
                    MessageEstRecuId = messageRecuId,
                    MessageId = messageId,
                    ConversationId = conversationId,
                    SenderId = senderId,
                    Date = date,
                    SentByCurrentUser = senderId == _currentUserId,
                    EstConforme = estConforme,
                    PhotoId = photoId,
                    Description = description,
                    //essageEnvoieColisId = messageEnvoieId
                };
                _selectedConversation.ListMessages?.Add(newRecu);
                NotifyStateChanged();
                OnMessageReceivedUI?.Invoke();
            }
        }
        else
        {
            var conv = _conversations.FirstOrDefault(c => c.ConversationId == conversationId);
            if (conv != null)
            {
                conv.LastMessage = estConforme ? "Colis reçu conforme" : "Colis non conforme";
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
        _signalRService.OnPaymentReceived -= OnPaymentReceivedFromSignalR;
        _signalRService.OnColisEnvoyeReceived -= OnColisEnvoyeReceivedFromSignalR;
        _signalRService.OnColisRecuReceived -= OnColisRecuReceivedFromSignalR;
        _signalRService.OnPaymentCancelled -= OnPaymentCancelledFromSignalR;
        _typingDisplayTimer?.Dispose();
    }
}