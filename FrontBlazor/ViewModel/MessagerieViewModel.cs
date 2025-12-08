using System.Collections.ObjectModel;
using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;
using Microsoft.AspNetCore.Components;

namespace FrontBlazor.ViewModel;

public class MessagerieViewModel : ComponentBase
{
    private readonly IConversationService<Conversation> _conversationService;
    private readonly IAuthService _authService;
    private readonly IMessageService<Message> _messageService;
    public bool IsLoading { get; set; } 
    public string? ErrorMessage { get; set; }
    
    public MessagerieViewModel(IConversationService<Conversation> conversationService, IAuthService authService, IMessageService<Message> messageService)
    {
        _conversationService = conversationService;
        _authService = authService;
        _messageService = messageService;
    }

    public ObservableCollection<Conversation> conversations { get; private set; }
        = new();

    public Conversation conv { get; private set; }
    public int? SelectedConversationId { get; private set; }
    public string NewMessage { get; set; } = string.Empty;
    public Utilisateur CurrentUser;
    
    public async Task Load()
    {
        CurrentUser = await _authService.GetCurrentUserAsync();
        Console.WriteLine(CurrentUser.UtilisateurId);
        var data = await _conversationService.GetConversationsByUserId(CurrentUser.UtilisateurId);
        conversations = data != null
            ? new ObservableCollection<Conversation>(data)
            : new ObservableCollection<Conversation>();
        //StateHasChanged();
    }

    public async Task SelectedConversation(int id)
    {
        CurrentUser = await _authService.GetCurrentUserAsync();
        
        SelectedConversationId = id;
        var data = await _conversationService.GetConversationDetailById(id);

        if (data != null)
            Console.WriteLine(data.ListMessages.Count);
        else
            Console.WriteLine("Pas de messages ou conversation null");

        conv = data ?? new Conversation();
        if (conv.ListMessages == null)
            conv.ListMessages = new List<Message>();
    }

    public async Task SendMessage()
    {
        await _authService.GetCurrentUserAsync();
        if (string.IsNullOrWhiteSpace(NewMessage)) return;
        Message message = new Message
        {
            //Date = DateTime.Now,
            //Lu = false,
            Content = NewMessage,
            ImagesId = null,
            //SenderId = CurrentUser.UtilisateurId, // obligatoire
            ConversationId = conv.ConversationId,
            UtilisateurId = CurrentUser.UtilisateurId
        };

        await _messageService.PostMessageTexte(message);
        message.SentbyCurrentUser = true;
        message.Date = DateTime.Now;
        conv.ListMessages.Add(message);
        NewMessage = string.Empty;
        //StateHasChanged();
    }
}