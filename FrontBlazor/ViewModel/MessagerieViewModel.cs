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
        = new();
    public int? SelectedConversationId { get; private set; }
    public string NewMessage { get; set; } = string.Empty;
    public Utilisateur CurrentUser;
    
    public async Task Load()
    {
        CurrentUser = await _authService.GetCurrentUserAsync();
        var data = await _conversationService.GetConversationsByUserId(CurrentUser.UtilisateurId);
        conversations = new ObservableCollection<Conversation>(data);
        StateHasChanged();
    }

    public async Task SelectedConversation(int id)
    {
        SelectedConversationId = id;
        var conv = await _conversationService.GetConversationDetailById(id);
        StateHasChanged();
    }

    public async Task SendMessage()
    {
        if (string.IsNullOrWhiteSpace(NewMessage)) return;
        Message message = new Message
        {
           Date = DateTime.Now,
           Lu = false,
           Contenu = NewMessage,
           Utilisateur = CurrentUser,
           ConversationId = conv.ConversationId
        };

        await _messageService.PostMessageTexte(message);
    }
}