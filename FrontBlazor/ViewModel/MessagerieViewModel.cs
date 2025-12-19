using System.Collections.ObjectModel;
using Shared.DTO;
using Shared.DTO.Conversation;
using Shared.DTO.Message;
using Shared.DTO.Utilisateur;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace FrontBlazor.ViewModel;

public class MessagerieViewModel : ComponentBase, IDisposable
{
    private readonly IConversationService<ConversationDTO> _conversationService;
    private readonly IAuthService _authService;
    private readonly IMessageService<MessageDTO> _messageService;
    public readonly ISignalRService _signalRService;
    private readonly NavigationManager _nav;

    public ObservableCollection<ConversationDTO> Conversations { get; private set; } = new();
    public ConversationDTO? SelectedConversation { get; private set; }
    public int? SelectedConversationId { get; private set; }
    public UtilisateurDTO? CurrentUser { get; private set; }

    public string NewMessage { get; set; } = "";
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
        IConversationService<ConversationDTO> conversationService,
        IAuthService authService,
        IMessageService<MessageDTO> messageService,
        NavigationManager nav,
        ISignalRService signalRService)
    {
        _conversationService = conversationService;
        _authService = authService;
        _nav = nav;
        _messageService = messageService;
        _signalRService = signalRService;

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
        Conversations = data != null ? new ObservableCollection<ConversationDTO>(data) : new ObservableCollection<ConversationDTO>();

        await _signalRService.StartAsync();
        
        foreach (var c in Conversations)
        {
            await _signalRService.JoinConversation(c.ConversationId);
            Console.WriteLine($"[VM] 🔗 Auto-joined conversation {c.ConversationId}");
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
                conv.ListMessages = new ObservableCollection<MessageDTO>();
            else if (conv.ListMessages is not ObservableCollection<MessageDTO>)
                conv.ListMessages = new ObservableCollection<MessageDTO>(conv.ListMessages);
        }
        else
        {
            SelectedConversation = new ConversationDTO { ListMessages = new ObservableCollection<MessageDTO>() };
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
        
        // ✅ CORRECTION 1 : Marquer les messages REÇUS comme lus LOCALEMENT
        if (SelectedConversation?.ListMessages != null)
        {
            var unreadReceivedMessages = SelectedConversation.ListMessages
                .Where(m => m.SentByCurrentUser == false && m.Lu == false)
                .ToList();

            foreach (var msg in unreadReceivedMessages)
            {
                try
                {
                    // Appeler l'API pour mettre à jour en base
                    await _messageService.MaskAsRead(msg.MessageId);
                    
                    // Mettre à jour localement seulement si l'API a réussi
                    msg.Lu = true;
                    
                    Console.WriteLine($"[VM] 📖 Marked received message as read: {msg.MessageId}");
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

        Console.WriteLine($"[VM] Joined conversation {conversationId} and marked as read");

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
            var message = new MessageTextDTO
            {
                Content = content,
                ConversationId = SelectedConversation.ConversationId,
                UtilisateurId = CurrentUser!.UtilisateurId,
                Date = DateTime.Now,
                SentByCurrentUser = true,
                Lu = false // ✅ Pas encore lu par l'autre
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
        Console.WriteLine($"[VM] HandleMessageReceived: conv={conversationId}, sender={senderId}, current={SelectedConversationId}");
    
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
                var newMessage = new MessageTextDTO
                {
                    Content = message,
                    UtilisateurId = senderId,
                    Date = date,
                    ConversationId = conversationId,
                    SentByCurrentUser = senderId == CurrentUser?.UtilisateurId,
                    Lu = false // ✅ Nouveau message non lu
                };

                SelectedConversation.ListMessages?.Add(newMessage);

                var previewConv = Conversations.FirstOrDefault(c => c.ConversationId == conversationId);
                if (previewConv != null)
                {
                    previewConv.LastMessage = message;
                }

                Console.WriteLine($"[VM] Message added to current conversation. Total: {SelectedConversation.ListMessages?.Count}");
                
                // ✅ CORRECTION 2 : Marquer automatiquement comme lu si on est dans la conversation
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
                        
                        Console.WriteLine($"[VM] Auto-marked new message as read");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[VM] ❌ Error auto-marking message as read: {ex.Message}");
                    }
                }
            }
            else
            {
                Console.WriteLine("[VM] Message already exists, skipping");
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

                Console.WriteLine($"[VM] Marked conversation {conversationId} as having new messages and moved to top");
            }

            NotifyStateChanged();
        }
    }

    private void HandleMessagesRead(int conversationId, int userId)
    {
        Console.WriteLine($"[VM] 📖 HandleMessagesRead: conv={conversationId}, userId={userId}, currentUser={CurrentUser?.UtilisateurId}");
    
        // ✅ CORRECTION 3 : L'autre utilisateur a lu nos messages
        if (userId != CurrentUser?.UtilisateurId)
        {
            ConversationDTO? targetConv = null;
        
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
                    .Where(m => m.UtilisateurId == CurrentUser!.UtilisateurId && m.SentByCurrentUser == true && m.Lu == false)
                    .ToList();
            
                if (mySentMessages.Any())
                {
                    foreach (var m in mySentMessages)
                    {
                        m.Lu = true;
                        Console.WriteLine($"[VM] ✅ Marked my sent message as read: '{m.Content?.Substring(0, Math.Min(20, m.Content?.Length ?? 0))}'");
                    }
                    Console.WriteLine($"[VM] 📖 MessagesRead: marked {mySentMessages.Count} messages as read in conv {conversationId}");
                    NotifyStateChanged();
                }
                else
                {
                    Console.WriteLine($"[VM] No unread sent messages found in conv {conversationId}");
                }
            }
        }
        else
        {
            Console.WriteLine($"[VM] MessagesRead ignored: it's my own read notification");
        }
    }

    private void HandleUserTyping(int conversationId, int userId, string userName)
    {
        Console.WriteLine($"[VM] ⌨️ HandleUserTyping: conv={conversationId}, user={userId}, name={userName}, selectedConv={SelectedConversationId}");
        
        // ✅ CORRECTION 4 : Ne pas afficher si c'est nous ou si ce n'est pas la conversation active
        if (SelectedConversationId != conversationId || userId == CurrentUser?.UtilisateurId)
        {
            Console.WriteLine($"[VM] Typing notification ignored");
            return;
        }

        // ✅ Afficher le nom de l'interlocuteur
        IsTyping = true;
        TypingUserName = userName;
        Console.WriteLine($"[VM] ✅ Showing typing indicator for {userName}");
        NotifyStateChanged();

        // Arrêter l'indicateur après 3 secondes
        _typingDisplayTimer?.Dispose();
        _typingDisplayTimer = new System.Threading.Timer(_ =>
        {
            IsTyping = false;
            TypingUserName = "";
            Console.WriteLine($"[VM] Hiding typing indicator");
            NotifyStateChanged();
        }, null, 3000, Timeout.Infinite);
    }

    public void HandleTyping(KeyboardEventArgs e)
    {
        if (SelectedConversation == null || CurrentUser == null)
            return;

        // Réinitialiser le timer
        _typingTimer?.Dispose();
        _typingTimer = new System.Threading.Timer(_ =>
        {
            _typingNotified = false;
        }, null, 2000, Timeout.Infinite);

        // Si on a déjà notifié récemment, ne pas re-notifier
        if (_typingNotified)
            return;

        _typingNotified = true;
        
        Console.WriteLine($"[VM] 📝 Notifying typing for conversation {SelectedConversation.ConversationId}");
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
}
// using System.Collections.ObjectModel;
// using Shared.DTO;
// using FrontBlazor.Services.GenericIServices;
// using FrontBlazor.Services.Interfaces;
// using Microsoft.AspNetCore.Components;
// using Microsoft.AspNetCore.Components.Web;
//
// namespace FrontBlazor.ViewModel;
//
// public class MessagerieViewModel : ComponentBase, IDisposable
// {
//     private readonly IConversationService<Conversation> _conversationService;
//     private readonly IAuthService _authService;
//     private readonly IMessageService<Message> _messageService;
//     public readonly ISignalRService _signalRService;
//     private readonly NavigationManager _nav;
//
//     public ObservableCollection<Conversation> Conversations { get; private set; } = new();
//     public Conversation? SelectedConversation { get; private set; }
//     public int? SelectedConversationId { get; private set; }
//     public Utilisateur? CurrentUser { get; private set; }
//
//     public string NewMessage { get; set; } = "";
//     public bool IsLoading { get; private set; } = false;
//     public bool IsTyping { get; private set; } = false;
//     public string TypingUserName { get; private set; } = "";
//
//     public ElementReference MessagesContainer;
//     public event Action? OnChange;
//     public event Action? OnMessageReceivedUI; 
//
//     private System.Threading.Timer? _typingTimer;
//     private System.Threading.Timer? _typingDisplayTimer;
//     private bool _typingNotified = false;
//
//     public MessagerieViewModel(
//         IConversationService<Conversation> conversationService,
//         IAuthService authService,
//         IMessageService<Message> messageService,
//         NavigationManager nav,
//         ISignalRService signalRService)
//     {
//         _conversationService = conversationService;
//         _authService = authService;
//         _nav = nav;
//         _messageService = messageService;
//         _signalRService = signalRService;
//
//         _signalRService.OnMessageReceived += HandleMessageReceived;
//         _signalRService.OnUserTyping += HandleUserTyping;
//         _signalRService.OnMessagesRead += HandleMessagesRead;
//     }
//
//     public async Task LoadAsync()
//     {
//         CurrentUser = await _authService.GetCurrentUserAsync();
//         if (CurrentUser == null)
//         {
//             _nav.NavigateTo("/");
//         }
//     }
//     
//     private void NotifyStateChanged() => OnChange?.Invoke();
//
//     public async Task LoadConversationsAsync()
//     {
//         IsLoading = true;
//         NotifyStateChanged();
//
//         CurrentUser = await _authService.GetCurrentUserAsync();
//         if (CurrentUser == null)
//         {
//             IsLoading = false;
//             NotifyStateChanged();
//             return;
//         }
//
//         var data = await _conversationService.GetConversationsByUserId(CurrentUser.UtilisateurId);
//         Conversations = data != null ? new ObservableCollection<Conversation>(data) : new ObservableCollection<Conversation>();
//
//         await _signalRService.StartAsync();
//         
//         foreach (var c in Conversations)
//         {
//             await _signalRService.JoinConversation(c.ConversationId);
//             Console.WriteLine($"[VM] 🔗 Auto-joined conversation {c.ConversationId}");
//         }
//
//         IsLoading = false;
//         NotifyStateChanged();
//     }
//
//     public async Task SelectConversationAsync(int conversationId)
//     {
//         if (CurrentUser == null)
//             CurrentUser = await _authService.GetCurrentUserAsync();
//
//         SelectedConversationId = conversationId;
//         var conv = await _conversationService.GetConversationDetailById(conversationId);
//
//         if (conv != null)
//         {
//             SelectedConversation = conv;
//             if (conv.ListMessages == null)
//                 conv.ListMessages = new ObservableCollection<Message>();
//             else if (conv.ListMessages is not ObservableCollection<Message>)
//                 conv.ListMessages = new ObservableCollection<Message>(conv.ListMessages);
//         }
//         else
//         {
//             SelectedConversation = new Conversation { ListMessages = new ObservableCollection<Message>() };
//         }
//
//         var listConv = Conversations.FirstOrDefault(c => c.ConversationId == conversationId);
//         if (listConv != null)
//         {
//             listConv.HasNewMessages = false;
//
//             if (SelectedConversation != null && !string.IsNullOrEmpty(SelectedConversation.LastMessage))
//             {
//                 listConv.LastMessage = SelectedConversation.LastMessage;
//             }
//         }
//
//         if (SelectedConversation != null)
//             SelectedConversation.HasNewMessages = false;
//         
//         // ✅ CORRECTION 1 : Marquer les messages REÇUS comme lus LOCALEMENT
//         if (SelectedConversation?.ListMessages != null)
//         {
//             var unreadReceivedMessages = SelectedConversation.ListMessages
//                 .Where(m => m.SentbyCurrentUser == false && m.Lu == false)
//                 .ToList();
//
//             foreach (var msg in unreadReceivedMessages)
//             {
//                 // Mettre à jour localement
//                 msg.Lu = true;
//                 
//                 // Appeler l'API pour mettre à jour en base
//                 await _messageService.MaskAsRead(msg.MessageId);
//                 
//                 Console.WriteLine($"[VM] 📖 Marked received message as read: {msg.MessageId}");
//             }
//         }
//         
//         // ✅ Notifier SignalR que j'ai lu les messages
//         await _signalRService.MarkMessagesAsRead(conversationId, CurrentUser!.UtilisateurId);
//
//         Console.WriteLine($"[VM] Joined conversation {conversationId} and marked as read");
//
//         NotifyStateChanged();
//     }
//
//     public async Task SendMessageAsync()
//     {
//         if (SelectedConversation == null || string.IsNullOrWhiteSpace(NewMessage))
//             return;
//         
//         var content = NewMessage.Trim();
//         NewMessage = "";
//         NotifyStateChanged();
//         
//         try
//         {
//             var message = new Message
//             {
//                 Content = content,
//                 ConversationId = SelectedConversation.ConversationId,
//                 UtilisateurId = CurrentUser!.UtilisateurId,
//                 Date = DateTime.Now,
//                 SentbyCurrentUser = true,
//                 Lu = false // ✅ Pas encore lu par l'autre
//             };
//         
//             await _messageService.PostMessageTexte(message);
//             
//             var conv = Conversations.FirstOrDefault(c => c.ConversationId == SelectedConversation.ConversationId);
//             if (conv != null)
//             {
//                 conv.LastMessage = content;
//                 conv.HasNewMessages = false;
//
//                 try
//                 {
//                     Conversations.Remove(conv);
//                     Conversations.Insert(0, conv);
//                 }
//                 catch (Exception ex)
//                 {
//                     Console.WriteLine($"[VM] Error moving conversation to top after sending: {ex.Message}");
//                 }
//             }
//         }
//         catch (Exception ex)
//         {
//             Console.WriteLine($"Erreur envoi message: {ex.Message}");
//         }
//         finally
//         {
//             NotifyStateChanged();
//         }
//     }
//
//     private async void HandleMessageReceived(int conversationId, int senderId, string message, DateTime date)
//     {
//         Console.WriteLine($"[VM] HandleMessageReceived: conv={conversationId}, sender={senderId}, current={SelectedConversationId}");
//     
//         if (SelectedConversation != null && SelectedConversation.ConversationId == conversationId)
//         {
//             var exists = SelectedConversation.ListMessages?.Any(m =>
//                 m.UtilisateurId == senderId &&
//                 m.Content == message &&
//                 m.Date.HasValue &&
//                 Math.Abs((m.Date.Value - date).TotalSeconds) < 2
//             ) ?? false;
//
//             if (!exists)
//             {
//                 var newMessage = new Message
//                 {
//                     Content = message,
//                     UtilisateurId = senderId,
//                     Date = date,
//                     ConversationId = conversationId,
//                     SentbyCurrentUser = senderId == CurrentUser?.UtilisateurId,
//                     Lu = false // ✅ Nouveau message non lu
//                 };
//
//                 SelectedConversation.ListMessages?.Add(newMessage);
//
//                 var previewConv = Conversations.FirstOrDefault(c => c.ConversationId == conversationId);
//                 if (previewConv != null)
//                 {
//                     previewConv.LastMessage = message;
//                 }
//
//                 Console.WriteLine($"[VM] Message added to current conversation. Total: {SelectedConversation.ListMessages?.Count}");
//                 
//                 // ✅ CORRECTION 2 : Marquer automatiquement comme lu si on est dans la conversation
//                 if (senderId != CurrentUser?.UtilisateurId)
//                 {
//                     // Mettre à jour localement
//                     newMessage.Lu = true;
//                     
//                     // Appeler l'API
//                     await _messageService.MaskAsRead(newMessage.MessageId);
//                     
//                     // Notifier SignalR
//                     await _signalRService.MarkMessagesAsRead(conversationId, CurrentUser!.UtilisateurId);
//                     
//                     Console.WriteLine($"[VM] Auto-marked new message as read");
//                 }
//             }
//             else
//             {
//                 Console.WriteLine("[VM] Message already exists, skipping");
//             }
//
//             NotifyStateChanged();
//             OnMessageReceivedUI?.Invoke();
//         }
//         else
//         {
//             var conv = Conversations.FirstOrDefault(c => c.ConversationId == conversationId);
//             if (conv != null)
//             {
//                 conv.LastMessage = message;
//                 conv.HasNewMessages = true;
//
//                 try
//                 {
//                     Conversations.Remove(conv);
//                     Conversations.Insert(0, conv);
//                 }
//                 catch (Exception ex)
//                 {
//                     Console.WriteLine($"[VM] Error moving conversation to top: {ex.Message}");
//                 }
//
//                 Console.WriteLine($"[VM] Marked conversation {conversationId} as having new messages and moved to top");
//             }
//
//             NotifyStateChanged();
//         }
//     }
//
//     private void HandleMessagesRead(int conversationId, int userId)
//     {
//         Console.WriteLine($"[VM] 📖 HandleMessagesRead: conv={conversationId}, userId={userId}, currentUser={CurrentUser?.UtilisateurId}");
//     
//         // ✅ CORRECTION 3 : L'autre utilisateur a lu nos messages
//         if (userId != CurrentUser?.UtilisateurId)
//         {
//             Conversation? targetConv = null;
//         
//             if (SelectedConversationId == conversationId && SelectedConversation != null)
//             {
//                 targetConv = SelectedConversation;
//             }
//             else
//             {
//                 targetConv = Conversations.FirstOrDefault(c => c.ConversationId == conversationId);
//             }
//
//             if (targetConv?.ListMessages != null)
//             {
//                 // Marquer MES messages envoyés comme lus
//                 var mySentMessages = targetConv.ListMessages
//                     .Where(m => m.UtilisateurId == CurrentUser!.UtilisateurId && m.SentbyCurrentUser == true && m.Lu == false)
//                     .ToList();
//             
//                 if (mySentMessages.Any())
//                 {
//                     foreach (var m in mySentMessages)
//                     {
//                         m.Lu = true;
//                         Console.WriteLine($"[VM] ✅ Marked my sent message as read: '{m.Content?.Substring(0, Math.Min(20, m.Content?.Length ?? 0))}'");
//                     }
//                     Console.WriteLine($"[VM] 📖 MessagesRead: marked {mySentMessages.Count} messages as read in conv {conversationId}");
//                     NotifyStateChanged();
//                 }
//                 else
//                 {
//                     Console.WriteLine($"[VM] No unread sent messages found in conv {conversationId}");
//                 }
//             }
//         }
//         else
//         {
//             Console.WriteLine($"[VM] MessagesRead ignored: it's my own read notification");
//         }
//     }
//
//     private void HandleUserTyping(int conversationId, int userId, string userName)
//     {
//         Console.WriteLine($"[VM] ⌨️ HandleUserTyping: conv={conversationId}, user={userId}, name={userName}, selectedConv={SelectedConversationId}");
//         
//         // ✅ CORRECTION 4 : Ne pas afficher si c'est nous ou si ce n'est pas la conversation active
//         if (SelectedConversationId != conversationId || userId == CurrentUser?.UtilisateurId)
//         {
//             Console.WriteLine($"[VM] Typing notification ignored");
//             return;
//         }
//
//         // ✅ Afficher le nom de l'interlocuteur
//         IsTyping = true;
//         TypingUserName = userName;
//         Console.WriteLine($"[VM] ✅ Showing typing indicator for {userName}");
//         NotifyStateChanged();
//
//         // Arrêter l'indicateur après 3 secondes
//         _typingDisplayTimer?.Dispose();
//         _typingDisplayTimer = new System.Threading.Timer(_ =>
//         {
//             IsTyping = false;
//             TypingUserName = "";
//             Console.WriteLine($"[VM] Hiding typing indicator");
//             NotifyStateChanged();
//         }, null, 3000, Timeout.Infinite);
//     }
//
//     public void HandleTyping(KeyboardEventArgs e)
//     {
//         if (SelectedConversation == null || CurrentUser == null)
//             return;
//
//         // Réinitialiser le timer
//         _typingTimer?.Dispose();
//         _typingTimer = new System.Threading.Timer(_ =>
//         {
//             _typingNotified = false;
//         }, null, 2000, Timeout.Infinite);
//
//         // Si on a déjà notifié récemment, ne pas re-notifier
//         if (_typingNotified)
//             return;
//
//         _typingNotified = true;
//         
//         Console.WriteLine($"[VM] 📝 Notifying typing for conversation {SelectedConversation.ConversationId}");
//         _ = _signalRService.NotifyTyping(SelectedConversation.ConversationId, CurrentUser.UtilisateurId, CurrentUser.Login ?? "Utilisateur");
//     }
//
//     public void Dispose()
//     {
//         _signalRService.OnMessageReceived -= HandleMessageReceived;
//         _signalRService.OnUserTyping -= HandleUserTyping;
//         _signalRService.OnMessagesRead -= HandleMessagesRead;
//         _typingTimer?.Dispose();
//         _typingDisplayTimer?.Dispose();
//     }
// }