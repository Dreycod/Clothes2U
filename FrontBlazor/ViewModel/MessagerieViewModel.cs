using System.Collections.ObjectModel;
using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;

namespace FrontBlazor.ViewModel;

public class MessagerieViewModel : IDisposable
{
    private readonly IConversationService<Conversation> _conversationService;
    private readonly IAuthService _authService;
    private readonly IMessageService<Message> _messageService;
    public readonly ISignalRService _signalRService;

    public ObservableCollection<Conversation> Conversations { get; private set; } = new();
    public Conversation? SelectedConversation { get; private set; }
    public int? SelectedConversationId { get; private set; }
    public Utilisateur? CurrentUser { get; private set; }

    public string NewMessage { get; set; } = "";
    public bool IsLoading { get; private set; } = false;
    public bool IsTyping { get; private set; } = false;

    public ElementReference MessagesContainer;
    public event Action? OnChange;
    
    public event Action? OnMessageReceivedUI; 

    private System.Threading.Timer? _typingTimer;
    private bool _typingNotified = false;

    public MessagerieViewModel(
        IConversationService<Conversation> conversationService,
        IAuthService authService,
        IMessageService<Message> messageService,
        ISignalRService signalRService)
    {
        _conversationService = conversationService;
        _authService = authService;
        _messageService = messageService;
        _signalRService = signalRService;

        _signalRService.OnMessageReceived += HandleMessageReceived;
        _signalRService.OnUserTyping += HandleUserTyping;
        _signalRService.OnMessagesRead += HandleMessagesRead;
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
        // S'abonner aux événements PropertyChanged de chaque conversation
        foreach (var conv in Conversations)
        {
            await _signalRService.JoinConversation(conv.ConversationId);
            Console.WriteLine($"[VM] Joined conversation {conv.ConversationId}");
            //conv.PropertyChanged += OnConversationPropertyChanged;
        }

        
        //NotifyStateChanged();

        IsLoading = false;
        NotifyStateChanged();
    }

    // Gestionnaire pour les changements de propriétés des conversations
    private void OnConversationPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        Console.WriteLine($"[VM] 🔔 Property '{e.PropertyName}' changed on conversation");
        if (e.PropertyName == nameof(Conversation.HasNewMessages) || e.PropertyName == nameof(Conversation.LastMessage))
        {
            // Forcer le rafraîchissement de l'UI
            Console.WriteLine($"[VM] 🔄 Forcing UI refresh due to {e.PropertyName} change");
            NotifyStateChanged();
        }
    }

    public async Task SelectConversationAsync(int conversationId)
    {
        if (CurrentUser == null)
            CurrentUser = await _authService.GetCurrentUserAsync();

        // Quitter l'ancienne conversation si elle existe

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
        
        // Reset HasNewMessages pour la conversation sélectionnée
        var convInList = Conversations.FirstOrDefault(c => c.ConversationId == conversationId);
        if (convInList != null)
        {
            convInList.HasNewMessages = false;
            Console.WriteLine($"[VM] ✅ Reset HasNewMessages for conversation {conversationId}");
        }

        // Rejoindre la nouvelle conversation
        await _signalRService.JoinConversation(conversationId);
        Console.WriteLine($"[VM] Joined conversation {conversationId}");

        NotifyStateChanged();
    }

    public async Task SendMessageAsync()
    {
        if (SelectedConversation == null || string.IsNullOrWhiteSpace(NewMessage))
            return;
        
        var content = NewMessage.Trim();
        
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
                SentbyCurrentUser = true
            };
        
            await _messageService.PostMessageTexte(message);
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
        Console.WriteLine($"[VM] ===== HandleMessageReceived =====");
        Console.WriteLine($"[VM]   ConversationId: {conversationId}");
        Console.WriteLine($"[VM]   SenderId: {senderId}");
        Console.WriteLine($"[VM]   CurrentUserId: {CurrentUser?.UtilisateurId}");
        Console.WriteLine($"[VM]   SelectedConversationId: {SelectedConversationId}");
        Console.WriteLine($"[VM]   Is same conversation? {SelectedConversation != null && SelectedConversation.ConversationId == conversationId}");
    
        // Si c'est la conversation actuelle, ajouter le message
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
                    SentbyCurrentUser = senderId == CurrentUser?.UtilisateurId
                };

                SelectedConversation.ListMessages?.Add(newMessage);
                Console.WriteLine($"[VM] ✅ Message added to current conversation. Total: {SelectedConversation.ListMessages?.Count}");
            
                // Mettre à jour LastMessage dans la liste
                var convInList = Conversations.FirstOrDefault(c => c.ConversationId == conversationId);
                if (convInList != null)
                {
                    convInList.LastMessage = message;
                }
            
                NotifyStateChanged();
                OnMessageReceivedUI?.Invoke();
            }
        }
        else
        {
            // Message dans une autre conversation - marquer comme nouveau
            Console.WriteLine($"[VM] 📬 Message for OTHER conversation (current={SelectedConversationId})");

            var conv = Conversations.FirstOrDefault(c => c.ConversationId == conversationId);
            if (conv != null)
            {
                Console.WriteLine($"[VM] 🔍 Found conversation {conversationId}");
                Console.WriteLine($"[VM]    BEFORE: HasNewMessages = {conv.HasNewMessages}");

                // Utiliser la propriété pour déclencher INotifyPropertyChanged
                conv.HasNewMessages = true;
                conv.LastMessage = message;
                
                NotifyStateChanged();   // ← déjà présent, mais **obligatoire**
                OnMessageReceivedUI?.Invoke();

                Console.WriteLine($"[VM]    AFTER: HasNewMessages = {conv.HasNewMessages}");
                Console.WriteLine($"[VM] ✅ Conversation marked as unread");
            }
            else
            {
                Console.WriteLine($"[VM] ❌ Conversation {conversationId} NOT FOUND");
            }
        }
    }

    private void HandleMessagesRead(int conversationId, int userId)
    {
        if (userId == CurrentUser?.UtilisateurId && SelectedConversationId == conversationId)
        {
            foreach (var msg in SelectedConversation!.ListMessages.Where(m => m.UtilisateurId != userId))
            {
                msg.SentbyCurrentUser = true;
            }
            NotifyStateChanged();
        }
    }

    private void HandleUserTyping(int conversationId, int userId, string userName)
    {
        if (SelectedConversationId != conversationId || userId == CurrentUser?.UtilisateurId)
            return;

        IsTyping = true;
        NotifyStateChanged();

        Task.Delay(3000).ContinueWith(_ =>
        {
            IsTyping = false;
            NotifyStateChanged();
        });
    }

    public void HandleTyping(KeyboardEventArgs e)
    {
        if (SelectedConversation == null)
            return;

        _typingTimer?.Dispose();
        _typingTimer = new System.Threading.Timer(_ =>
        {
            _typingNotified = false;
        }, null, 2000, Timeout.Infinite);

        if (_typingNotified)
            return;

        _typingNotified = true;
        _ = _signalRService.NotifyTyping(SelectedConversation.ConversationId, CurrentUser!.UtilisateurId, "User");
    }

    public void Dispose()
    {
        // Se désabonner des événements PropertyChanged
        foreach (var conv in Conversations)
        {
            conv.PropertyChanged -= OnConversationPropertyChanged;
        }

        _signalRService.OnMessageReceived -= HandleMessageReceived;
        _signalRService.OnUserTyping -= HandleUserTyping;
        _signalRService.OnMessagesRead -= HandleMessagesRead;
        _typingTimer?.Dispose();
    }
}