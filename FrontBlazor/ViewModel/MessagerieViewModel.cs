using System.Collections.ObjectModel;
using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace FrontBlazor.ViewModel;

public class MessagerieViewModel : ComponentBase, IDisposable
{
    private readonly IConversationService<Conversation> _conversationService;
    private readonly IAuthService _authService;
    public readonly IMediasService<Photo> _mediaService;
    private readonly IMessageService<Message> _messageService;
    public readonly ISignalRService _signalRService;
    private readonly NavigationManager _nav;

    public ObservableCollection<Conversation> Conversations { get; private set; } = new();
    public Conversation? SelectedConversation { get; private set; }
    public int? SelectedConversationId { get; private set; }
    public Utilisateur? CurrentUser { get; private set; }

    public string NewMessage { get; set; } = "";
    public IBrowserFile? SelectedFile { get; set; }
    public bool IsLoading { get; private set; } = false;
    public bool IsTyping { get; private set; } = false;
    public string TypingUserName { get; private set; } = "";

    public ElementReference MessagesContainer;
    public event Action? OnChange;
    public event Action? OnMessageReceivedUI; 

    private System.Threading.Timer? _typingTimer;
    private System.Threading.Timer? _typingDisplayTimer;
    private bool _typingNotified = false;

    public MessagerieViewModel(
        IConversationService<Conversation> conversationService,
        IAuthService authService,
        IMessageService<Message> messageService,
        IMediasService<Photo> mediaService,
        NavigationManager nav,
        ISignalRService signalRService)
    {
        _conversationService = conversationService;
        _authService = authService;
        _nav = nav;
        _messageService = messageService;
        _signalRService = signalRService;
        _mediaService = mediaService;
        
        _signalRService.OnMessageReceived += HandleMessageReceived;
        _signalRService.OnUserTyping += HandleUserTyping;
        _signalRService.OnMessagesRead += HandleMessagesRead;
    }

    public async Task LoadAsync()
    {
        CurrentUser = await _authService.GetCurrentUserAsync();
        if (CurrentUser == null)
        {
            _nav.NavigateTo("/");
        }
    }
    
    
    private void NotifyStateChanged() => OnChange?.Invoke();

    public async Task LoadConversationsAsync()
    {
        IsLoading = true;
        NotifyStateChanged();

        CurrentUser = await _authService.GetCurrentUserAsync();
        if (CurrentUser == null)
        {
            IsLoading = false;
            NotifyStateChanged();
            return;
        }

        var data = await _conversationService.GetConversationsByUserId(CurrentUser.UtilisateurId);
        Conversations = data != null ? new ObservableCollection<Conversation>(data) : new ObservableCollection<Conversation>();

        await _signalRService.StartAsync();
        
        foreach (var c in Conversations)
        {
            await _signalRService.JoinConversation(c.ConversationId);
        }

        IsLoading = false;
        NotifyStateChanged();
    }

    public async Task SelectConversationAsync(int conversationId)
    {
        if (CurrentUser == null)
            CurrentUser = await _authService.GetCurrentUserAsync();

        SelectedConversationId = conversationId;
        var conv = await _conversationService.GetConversationDetailById(conversationId);

        if (conv != null)
        {
            SelectedConversation = conv;
            if (conv.ListMessages == null)
                conv.ListMessages = new ObservableCollection<Message>();
            else if (conv.ListMessages is not ObservableCollection<Message>)
                conv.ListMessages = new ObservableCollection<Message>(conv.ListMessages);
        }
        else
        {
            SelectedConversation = new Conversation { ListMessages = new ObservableCollection<Message>() };
        }

        var listConv = Conversations.FirstOrDefault(c => c.ConversationId == conversationId);
        if (listConv != null)
        {
            listConv.HasNewMessages = false;

            if (SelectedConversation != null && !string.IsNullOrEmpty(SelectedConversation.LastMessage))
            {
                listConv.LastMessage = SelectedConversation.LastMessage;
            }
        }

        if (SelectedConversation != null)
            SelectedConversation.HasNewMessages = false;
        
        if (SelectedConversation?.ListMessages != null)
        {
            var unreadReceivedMessages = SelectedConversation.ListMessages
                .Where(m => m.SentbyCurrentUser == false && m.Lu == false)
                .ToList();

            foreach (var msg in unreadReceivedMessages)
            {
                try
                {
                    // Appeler l'API pour mettre à jour en base
                    await _messageService.MaskAsRead(msg.MessageId);
                    
                    // Mettre à jour localement seulement si l'API a réussi
                    msg.Lu = true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[VM] ❌ Error marking message {msg.MessageId} as read: {ex.Message}");
                    // Continue avec les autres messages même si un échoue
                }
            }
        }
        
        // ✅ Notifier SignalR que j'ai lu les messages
        await _signalRService.MarkMessagesAsRead(conversationId, CurrentUser!.UtilisateurId);
        NotifyStateChanged();
    }

    public async Task SendMessageAsync()
    {
        if (SelectedConversation == null || string.IsNullOrWhiteSpace(NewMessage))
            return;
        
        var content = NewMessage.Trim();
        var images = SelectedFile;
        SelectedFile = null;
        NewMessage = "";
        NotifyStateChanged();
        
        try
        {
            var message = new Message
            {
                Content = content,
                ConversationId = SelectedConversation.ConversationId,
                UtilisateurId = CurrentUser!.UtilisateurId,
                Date = DateTime.Now,
                SentbyCurrentUser = true,
                Lu = false 
            };
        
            await _messageService.PostMessageTexte(message);
            
            var conv = Conversations.FirstOrDefault(c => c.ConversationId == SelectedConversation.ConversationId);
            if (conv != null)
            {
                conv.LastMessage = content;
                conv.HasNewMessages = false;

                try
                {
                    Conversations.Remove(conv);
                    Conversations.Insert(0, conv);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[VM] Error moving conversation to top after sending: {ex.Message}");
                }
            }
            
            var conversation = await _conversationService.GetConversationDetailById(conv.ConversationId);
            var lastMessages =  conversation.ListMessages.LastOrDefault();
            if (images != null && lastMessages.MessageId != null)
            {
                var bytes = await ConvertIBrowserFileToBytesAsync(images);
                var fileName = images.Name;

                await _mediaService.UploadPhotoMessageAsync(
                    (int)lastMessages.MessageId,
                    bytes,
                    fileName
                );
                
                var updatedConversation = await _conversationService.GetConversationDetailById(conv.ConversationId);
                var updatedMesssage = updatedConversation.ListMessages.FirstOrDefault(m => m.MessageId == lastMessages.MessageId);
                if (updatedMesssage != null)
                {
                    var localMessage = SelectedConversation.ListMessages?.FirstOrDefault(m => m.MessageId == lastMessages.MessageId);

                    if (localMessage != null)
                    {
                        localMessage.Photos = updatedMesssage.Photos;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur envoi message: {ex.Message}");
        }
        finally
        {
            NotifyStateChanged();
        }
    }

    private async void HandleMessageReceived(int conversationId, int senderId, string message, DateTime date)
    {
        if (SelectedConversation != null && SelectedConversation.ConversationId == conversationId)
        {
            var exists = SelectedConversation.ListMessages?.Any(m =>
                m.UtilisateurId == senderId &&
                m.Content == message &&
                m.Date.HasValue &&
                Math.Abs((m.Date.Value - date).TotalSeconds) < 2
            ) ?? false;

            if (!exists)
            {
                var newMessage = new Message
                {
                    Content = message,
                    UtilisateurId = senderId,
                    Date = date,
                    ConversationId = conversationId,
                    SentbyCurrentUser = senderId == CurrentUser?.UtilisateurId,
                    Lu = false // ✅ Nouveau message non lu
                };

                SelectedConversation.ListMessages?.Add(newMessage);

                var previewConv = Conversations.FirstOrDefault(c => c.ConversationId == conversationId);
                if (previewConv != null)
                {
                    previewConv.LastMessage = message;
                }
                
                // Marquer automatiquement comme lu si on est dans la conversation
                if (senderId != CurrentUser?.UtilisateurId)
                {
                    try
                    {
                        // Appeler l'API
                        await _messageService.MaskAsRead(newMessage.MessageId);
                        
                        // Mettre à jour localement seulement si l'API a réussi
                        newMessage.Lu = true;
                        
                        // Notifier SignalR
                        await _signalRService.MarkMessagesAsRead(conversationId, CurrentUser!.UtilisateurId);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[VM] ❌ Error auto-marking message as read: {ex.Message}");
                    }
                }
            }

            NotifyStateChanged();
            OnMessageReceivedUI?.Invoke();
        }
        else
        {
            var conv = Conversations.FirstOrDefault(c => c.ConversationId == conversationId);
            if (conv != null)
            {
                conv.LastMessage = message;
                conv.HasNewMessages = true;

                try
                {
                    Conversations.Remove(conv);
                    Conversations.Insert(0, conv);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[VM] Error moving conversation to top: {ex.Message}");
                }

            }

            NotifyStateChanged();
        }
    }

    private void HandleMessagesRead(int conversationId, int userId)
    {
        // L'autre utilisateur a lu nos messages
        if (userId != CurrentUser?.UtilisateurId)
        {
            Conversation? targetConv = null;
        
            if (SelectedConversationId == conversationId && SelectedConversation != null)
            {
                targetConv = SelectedConversation;
            }
            else
            {
                targetConv = Conversations.FirstOrDefault(c => c.ConversationId == conversationId);
            }

            if (targetConv?.ListMessages != null)
            {
                // Marquer MES messages envoyés comme lus
                var mySentMessages = targetConv.ListMessages
                    .Where(m => m.UtilisateurId == CurrentUser!.UtilisateurId && m.SentbyCurrentUser == true && m.Lu == false)
                    .ToList();
            
                if (mySentMessages.Any())
                {
                    foreach (var m in mySentMessages)
                    {
                        m.Lu = true;
                    }
                    NotifyStateChanged();
                }
                else
                {
                }
            }
        }
        else
        {
        }
    }

    private void HandleUserTyping(int conversationId, int userId, string userName)
    {
        // CORRECTION 4 : Ne pas afficher si c'est nous ou si ce n'est pas la conversation active
        if (SelectedConversationId != conversationId || userId == CurrentUser?.UtilisateurId)
        {
            return;
        }

        // Afficher le nom de l'interlocuteur
        IsTyping = true;
        TypingUserName = userName;
        NotifyStateChanged();

        // Arrêter l'indicateur après 3 secondes
        _typingDisplayTimer?.Dispose();
        _typingDisplayTimer = new System.Threading.Timer(_ =>
        {
            IsTyping = false;
            TypingUserName = "";
            NotifyStateChanged();
        }, null, 3000, Timeout.Infinite);
    }

    public void HandleTyping(KeyboardEventArgs e)
    {
        if (SelectedConversation == null || CurrentUser == null)
            return;

        _typingTimer?.Dispose();
        _typingTimer = new System.Threading.Timer(_ =>
        {
            _typingNotified = false;
        }, null, 2000, Timeout.Infinite);

        if (_typingNotified)
            return;

        _typingNotified = true;
        
        _ = _signalRService.NotifyTyping(SelectedConversation.ConversationId, CurrentUser.UtilisateurId, CurrentUser.Login ?? "Utilisateur");
    }

    public void Dispose()
    {
        _signalRService.OnMessageReceived -= HandleMessageReceived;
        _signalRService.OnUserTyping -= HandleUserTyping;
        _signalRService.OnMessagesRead -= HandleMessagesRead;
        _typingTimer?.Dispose();
        _typingDisplayTimer?.Dispose();
    }
    
    private async Task<byte[]> ConvertIBrowserFileToBytesAsync(IBrowserFile file)
    {
        using var ms = new MemoryStream();
        await file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024) // 10 MB par ex
            .CopyToAsync(ms);
        return ms.ToArray();
    }
}
