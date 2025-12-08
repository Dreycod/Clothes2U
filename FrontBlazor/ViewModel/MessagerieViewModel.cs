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
    public event Action? OnChange;
    
    public MessagerieViewModel(IConversationService<Conversation> conversationService, 
        IAuthService authService, 
        IMessageService<Message> messageService)
    {
        _conversationService = conversationService;
        _authService = authService;
        _messageService = messageService;
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
        // CurrentUser = await _authService.GetCurrentUserAsync();
        // Console.WriteLine(CurrentUser.UtilisateurId);
        // var data = await _conversationService.GetConversationsByUserId(CurrentUser.UtilisateurId);
        // conversations = data != null
        //     ? new ObservableCollection<Conversation>(data)
        //     : new ObservableCollection<Conversation>();
        // //StateHasChanged();
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
            }
            else
            {
                Console.WriteLine("Pas de messages ou conversation null");
                conv = new Conversation 
                { 
                    ListMessages = new ObservableCollection<Message>() 
                };
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
        // CurrentUser = await _authService.GetCurrentUserAsync();
        //
        // SelectedConversationId = id;
        // var data = await _conversationService.GetConversationDetailById(id);
        //
        // if (data != null)
        //     Console.WriteLine(data.ListMessages.Count);
        // else
        //     Console.WriteLine("Pas de messages ou conversation null");
        //
        // conv = data ?? new Conversation();
        // if (conv.ListMessages == null)
        //     conv.ListMessages = new ObservableCollection<Message>();
        // else if (conv.ListMessages is not ObservableCollection<Message>)
        //     conv.ListMessages = new ObservableCollection<Message>(conv.ListMessages);
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
                Date = DateTime.Now, // Ajouter la date pour l'affichage immédiat
                SentbyCurrentUser = true // Pour l'affichage correct
            };

            await _messageService.PostMessageTexte(message);
            
            // Ajouter le message localement pour affichage immédiat
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
            
            // Optionnel : recharger depuis le serveur pour synchroniser
            // await SelectedConversation(conv.ConversationId);
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
        // await _authService.GetCurrentUserAsync();
        //
        // if (string.IsNullOrWhiteSpace(NewMessage)) return;
        //
        // Message message = new Message
        // {
        //     Content = NewMessage,
        //     ImagesId = null,
        //     ConversationId = conv.ConversationId,
        //     UtilisateurId = CurrentUser.UtilisateurId
        // };
        //
        // var success = await _messageService.PostMessageTexte(message);
        // Console.WriteLine("hahhahhahah"+success);
        // if (success. == true)
        // {
        //     // Recharger la conversation complète depuis le serveur
        //     await SelectedConversation(conv.ConversationId);
        //     NewMessage = string.Empty;
        // }
    }
}