
using System.Collections.ObjectModel;
using FrontBlazor.Models;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using Microsoft.AspNetCore.Components;

namespace FrontBlazor.ViewModel;

public class MessagerieViewModel : ComponentBase
{
    private readonly IConversationService<Conversation> _conversationService;
    private readonly IAuthService _authService;
    private readonly IMessageService<Message> _messageService;
    private readonly ChatSignalRService _signalRService;
    
    public bool IsLoading { get; set; } 
    public string? ErrorMessage { get; set; }
    public event Action? OnChange;
    
    public MessagerieViewModel(
        IConversationService<Conversation> conversationService, 
        IAuthService authService, 
        IMessageService<Message> messageService,
        ChatSignalRService signalRService)
    {
        _conversationService = conversationService;
        _authService = authService;
        _messageService = messageService;
        _signalRService = signalRService;
    }

    public ObservableCollection<Conversation> conversations { get; private set; } = new();
    public Conversation? conv { get; private set; }
    public int? SelectedConversationId { get; private set; }
    public string NewMessage { get; set; } = string.Empty;
    public Utilisateur? CurrentUser { get; private set; }
    
    private void NotifyStateChanged() => OnChange?.Invoke();
    
    public async Task Load()
    {
        IsLoading = true;
        ErrorMessage = null;
        
        try
        {
            CurrentUser = await _authService.GetCurrentUserAsync();
            if (CurrentUser == null)
            {
                ErrorMessage = "Utilisateur non connecté";
                return;
            }
            
            Console.WriteLine($"Loading conversations for user: {CurrentUser.UtilisateurId}");
            var data = await _conversationService.GetConversationsByUserId(CurrentUser.UtilisateurId);
            
            conversations = data != null
                ? new ObservableCollection<Conversation>(data)
                : new ObservableCollection<Conversation>();
                
            NotifyStateChanged();
            _signalRService.OnMessageReceived += OnNewMessageReceived;
            await _signalRService.StartAsync("http://localhost:5096/chatHub");
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur lors du chargement: {ex.Message}";
            Console.WriteLine($"Error in Load: {ex}");
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    public async Task SelectedConversation(int id)
    {
        IsLoading = true;
        ErrorMessage = null;
        
        try
        {
            if (CurrentUser == null)
            {
                CurrentUser = await _authService.GetCurrentUserAsync();
            }
            
            SelectedConversationId = id;
            var data = await _conversationService.GetConversationDetailById(id);

            if (data != null)
            {
                Console.WriteLine($"Loaded {data.ListMessages?.Count ?? 0} messages");
                conv = data;
                
                // S'assurer que ListMessages est une ObservableCollection
                if (conv.ListMessages == null)
                    conv.ListMessages = new ObservableCollection<Message>();
                else if (conv.ListMessages is not ObservableCollection<Message>)
                    conv.ListMessages = new ObservableCollection<Message>(conv.ListMessages);
                await _signalRService.LeaveConversation(SelectedConversationId.Value);
            }
            else
            {
                Console.WriteLine("Pas de messages ou conversation null");
                conv = new Conversation 
                { 
                    ListMessages = new ObservableCollection<Message>() 
                };
                await _signalRService.JoinConversation(id);
            }
            
            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur lors du chargement de la conversation: {ex.Message}";
            Console.WriteLine($"Error in SelectedConversation: {ex}");
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    public async Task SendMessage()
    {
        if (CurrentUser == null)
        {
            CurrentUser = await _authService.GetCurrentUserAsync();
        }
        
        if (string.IsNullOrWhiteSpace(NewMessage)) return;
        if (conv == null || SelectedConversationId == null) return;
        
        IsLoading = true;
        ErrorMessage = null;
        
        try
        {
            Message message = new Message
            {
                Content = NewMessage,
                ImagesId = null,
                ConversationId = conv.ConversationId,
                UtilisateurId = CurrentUser.UtilisateurId,
                Date = DateTime.Now,
                SentbyCurrentUser = true 
            };

            await _messageService.PostMessageTexte(message);
            
            if (conv.ListMessages is ObservableCollection<Message> observableList)
            {
                observableList.Add(message);
            }
            else
            {
                conv.ListMessages.Add(message);
            }
            
            NewMessage = string.Empty;
            NotifyStateChanged();
            
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Erreur lors de l'envoi: {ex.Message}";
            Console.WriteLine($"Error in SendMessage: {ex}");
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }
    private void OnNewMessageReceived(int conversationId, Message message)
    {
        if (SelectedConversationId == conversationId && conv?.ListMessages != null)
        {
            if (conv.ListMessages is ObservableCollection<Message> observableList)
            {
                observableList.Add(message);
            }
            else
            {
                conv.ListMessages.Add(message);
            }
            
            NotifyStateChanged();
        }
    }
}