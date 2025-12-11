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
    private readonly IMessageService<Message> _messageService;
    private readonly ISignalRService _signalRService;
    private readonly Func<Task>? _refreshUi;
    private readonly NavigationManager _nav;

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
        ISignalRService signalRService,
        Func<Task>? refreshUi = null)
    {
        _conversationService = conversationService;
        _authService = authService;
        _messageService = messageService;
        _signalRService = signalRService;
        _refreshUi = refreshUi;

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
        NotifyStateChanged();

        IsLoading = false;
        NotifyStateChanged();
    }

    public async Task SelectConversationAsync(int conversationId)
    {
        if (CurrentUser == null)
            CurrentUser = await _authService.GetCurrentUserAsync();

        // Quitter l'ancienne conversation si elle existe
        if (SelectedConversationId.HasValue)
        {
            await _signalRService.LeaveConversation(SelectedConversationId.Value);
            Console.WriteLine($"[VM] Left conversation {SelectedConversationId.Value}");
        }

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
        //var filesToUpload = new List<IBrowserFile>(SelectedFiles);
        
        NewMessage = "";
        //SelectedFiles.Clear();
        NotifyStateChanged();
        
        //IsUploadingFiles = true;
        
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
            //SelectedConversation.ListMessages.Add(message);
        
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur envoi message: {ex.Message}");
        }
        finally
        {
            //IsUploadingFiles = false;
            NotifyStateChanged();
        }
    }

    private async void HandleMessageReceived(int conversationId, int senderId, string message, DateTime date)
    {
        Console.WriteLine($"[VM] HandleMessageReceived: conv={conversationId}, sender={senderId}, current={SelectedConversationId}");
    
        // Si c'est la conversation actuelle, ajouter le message
        if (SelectedConversation != null && SelectedConversation.ConversationId == conversationId)
        {
            // Vérifier si le message n'existe pas déjà (éviter les doublons)
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
                    UtilisateurId = senderId, // ← Utiliser UtilisateurId, pas SenderId
                    Date = date,
                    ConversationId = conversationId,
                    SentbyCurrentUser = senderId == CurrentUser?.UtilisateurId
                };

                SelectedConversation.ListMessages?.Add(newMessage);
                Console.WriteLine($"[VM] Message added to current conversation. Total: {SelectedConversation.ListMessages?.Count}");
            
                // 🔥 CRITIQUE : Notifier le changement
                NotifyStateChanged();
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
            // Message dans une autre conversation - marquer comme nouveau
            var conv = Conversations.FirstOrDefault(c => c.ConversationId == conversationId);
            if (conv != null)
            {
                conv.HasNewMessages = true;
                conv.LastMessage = conv.LastMessage;
                Console.WriteLine($"[VM] Marked conversation {conversationId} as having new messages");
            }
        
            NotifyStateChanged();
        }
        
    }

    private void HandleMessagesRead(int conversationId, int userId)
    {
        if (userId == CurrentUser?.UtilisateurId && SelectedConversationId == conversationId)
        {
            foreach (var msg in SelectedConversation!.ListMessages.Where(m => m.UtilisateurId != userId))
            {
                msg.SentbyCurrentUser = true; // Marque comme lu
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
        _signalRService.OnMessageReceived -= HandleMessageReceived;
        _signalRService.OnUserTyping -= HandleUserTyping;
        _signalRService.OnMessagesRead -= HandleMessagesRead;
        _typingTimer?.Dispose();
    }
}
//
// using System.Collections.ObjectModel;
// using FrontBlazor.Models;
// using FrontBlazor.Services;
// using FrontBlazor.Services.GenericIServices;
// using Microsoft.AspNetCore.Components;
//
// namespace FrontBlazor.ViewModel;
//
// public class MessagerieViewModel : ComponentBase
// {
//     private readonly IConversationService<Conversation> _conversationService;
//     private readonly IAuthService _authService;
//     private readonly IMessageService<Message> _messageService;
//     private readonly SignalRWebService _signalRService;
//     
//     public bool IsLoading { get; set; } 
//     public string? ErrorMessage { get; set; }
//     public event Action? OnChange;
//     
//     public MessagerieViewModel(
//         IConversationService<Conversation> conversationService, 
//         IAuthService authService, 
//         IMessageService<Message> messageService,
//         SignalRWebService signalRService)
//     {
//         _conversationService = conversationService;
//         _authService = authService;
//         _messageService = messageService;
//         _signalRService = signalRService;
//     }
//
//     public ObservableCollection<Conversation> conversations { get; private set; } = new();
//     public Conversation? conv { get; private set; }
//     public int? SelectedConversationId { get; private set; }
//     public string NewMessage { get; set; } = string.Empty;
//     public Utilisateur? CurrentUser { get; private set; }
//     
//     private void NotifyStateChanged() => OnChange?.Invoke();
//     
//     public async Task Load()
//     {
//         IsLoading = true;
//         ErrorMessage = null;
//         
//         try
//         {
//             CurrentUser = await _authService.GetCurrentUserAsync();
//             if (CurrentUser == null)
//             {
//                 ErrorMessage = "Utilisateur non connecté";
//                 return;
//             }
//             
//             Console.WriteLine($"Loading conversations for user: {CurrentUser.UtilisateurId}");
//             var data = await _conversationService.GetConversationsByUserId(CurrentUser.UtilisateurId);
//             
//             conversations = data != null
//                 ? new ObservableCollection<Conversation>(data)
//                 : new ObservableCollection<Conversation>();
//                 
//             NotifyStateChanged();
//             _signalRService.OnMessageReceived += OnNewMessageReceived;
//             await _signalRService.StartAsync("http://localhost:5096/chatHub");
//         }
//         catch (Exception ex)
//         {
//             ErrorMessage = $"Erreur lors du chargement: {ex.Message}";
//             Console.WriteLine($"Error in Load: {ex}");
//         }
//         finally
//         {
//             IsLoading = false;
//             NotifyStateChanged();
//         }
//     }
//
//     public async Task SelectedConversation(int id)
//     {
//         IsLoading = true;
//         ErrorMessage = null;
//         
//         try
//         {
//             if (CurrentUser == null)
//             {
//                 CurrentUser = await _authService.GetCurrentUserAsync();
//             }
//             
//             SelectedConversationId = id;
//             var data = await _conversationService.GetConversationDetailById(id);
//
//             if (data != null)
//             {
//                 Console.WriteLine($"Loaded {data.ListMessages?.Count ?? 0} messages");
//                 conv = data;
//                 
//                 // S'assurer que ListMessages est une ObservableCollection
//                 if (conv.ListMessages == null)
//                     conv.ListMessages = new ObservableCollection<Message>();
//                 else if (conv.ListMessages is not ObservableCollection<Message>)
//                     conv.ListMessages = new ObservableCollection<Message>(conv.ListMessages);
//                 await _signalRService.LeaveConversation(SelectedConversationId.Value);
//             }
//             else
//             {
//                 Console.WriteLine("Pas de messages ou conversation null");
//                 conv = new Conversation 
//                 { 
//                     ListMessages = new ObservableCollection<Message>() 
//                 };
//                 await _signalRService.JoinConversation(id);
//             }
//             
//             NotifyStateChanged();
//         }
//         catch (Exception ex)
//         {
//             ErrorMessage = $"Erreur lors du chargement de la conversation: {ex.Message}";
//             Console.WriteLine($"Error in SelectedConversation: {ex}");
//         }
//         finally
//         {
//             IsLoading = false;
//             NotifyStateChanged();
//         }
//     }
//
//     public async Task SendMessage()
//     {
//         if (CurrentUser == null)
//         {
//             CurrentUser = await _authService.GetCurrentUserAsync();
//         }
//         
//         if (string.IsNullOrWhiteSpace(NewMessage)) return;
//         if (conv == null || SelectedConversationId == null) return;
//         
//         IsLoading = true;
//         ErrorMessage = null;
//         
//         try
//         {
//             Message message = new Message
//             {
//                 Content = NewMessage,
//                 ImagesId = null,
//                 ConversationId = conv.ConversationId,
//                 UtilisateurId = CurrentUser.UtilisateurId,
//                 Date = DateTime.Now,
//                 SentbyCurrentUser = true 
//             };
//
//             await _messageService.PostMessageTexte(message);
//             
//             if (conv.ListMessages is ObservableCollection<Message> observableList)
//             {
//                 observableList.Add(message);
//             }
//             else
//             {
//                 conv.ListMessages.Add(message);
//             }
//             
//             NewMessage = string.Empty;
//             NotifyStateChanged();
//             
//         }
//         catch (Exception ex)
//         {
//             ErrorMessage = $"Erreur lors de l'envoi: {ex.Message}";
//             Console.WriteLine($"Error in SendMessage: {ex}");
//         }
//         finally
//         {
//             IsLoading = false;
//             NotifyStateChanged();
//         }
//     }
//     private void OnNewMessageReceived(int conversationId, Message message)
//     {
//         if (SelectedConversationId == conversationId && conv?.ListMessages != null)
//         {
//             if (conv.ListMessages is ObservableCollection<Message> observableList)
//             {
//                 observableList.Add(message);
//             }
//             else
//             {
//                 conv.ListMessages.Add(message);
//             }
//             
//             NotifyStateChanged();
//         }
//     }
//}