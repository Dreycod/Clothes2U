using System.Collections.ObjectModel;
using Shared.DTO;
using Shared.DTO.Conversation;
using Shared.DTO.Message;
using Shared.DTO.Utilisateur;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Shared.DTO.Photo;

namespace FrontBlazor.ViewModel;

public class MessagerieViewModel : ComponentBase, IDisposable
{
    private readonly IConversationService<ConversationDTO> _conversationService;
    private readonly IAuthService _authService;
    public readonly IMediasService _mediaService;
    private readonly IMessageService _messageService;
    public readonly ISignalRService _signalRService;
    private readonly NavigationManager _nav;

    public ObservableCollection<ConversationDTO> Conversations { get; private set; } = new();
    public ConversationDTO? SelectedConversation { get; private set; }
    public int? SelectedConversationId { get; private set; }
    public UtilisateurDTO? CurrentUser { get; private set; }

    public string NewMessage { get; set; } = "";
    public List<IBrowserFile> SelectedFile { get; set; }
    public List<(IBrowserFile File, string PreviewBase64)> SelectedFilePreviews { get; set; } = new();
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
        IMessageService messageService,
        IMediasService mediaService,
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
        _signalRService.OnProposalResponse += HandleProposalResponse;
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
            SelectedConversation = new ConversationDTO() { ListMessages = new ObservableCollection<MessageDTO>() };
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
                .Where(m => m.SentByCurrentUser == false && m.Lu == false)
                .ToList();

            foreach (var msg in unreadReceivedMessages)
            {
                try
                {
                    // Appeler l'API pour mettre à jour en base
                    await _messageService.MaskAsRead(msg.MessageId.Value);
                    
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
        var images = SelectedFile?.ToList();
        SelectedFile?.Clear();
        SelectedFilePreviews.Clear();
        NewMessage = "";
        NotifyStateChanged();
        
        try
        {
            List<PhotoUploadDTO>? photoDto = new List<PhotoUploadDTO>();
            if (images != null)
            {
                foreach (var image in images)
                {
                    var bytes = await ConvertIBrowserFileToBytesAsync(image);

                    var photo = new PhotoUploadDTO
                    {
                        FileName = image.Name,
                        ContentType = image.ContentType,
                        FileSize = image.Size,
                        Base64Data = Convert.ToBase64String(bytes)
                    };
                    photoDto.Add(photo);
                }
                
            }
                
            
            var message = new MessageTextePostDTO()
            {
                Content = content,
                ConversationId = SelectedConversation.ConversationId,
                UtilisateurId = CurrentUser!.UtilisateurId,
                Photos = photoDto
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
    
    // public async Task SendProposition(double newPrice)
    // {
    //     if (SelectedConversation.Prix * 0.7 > newPrice)
    //     {
    //         return;
    //     }
    //
    //     try
    //     {
    //         MessageDemandePostDTO messageDemandePostDto = new MessageDemandePostDTO
    //         {
    //             ConversationId = SelectedConversationId!.Value,
    //             PrixPropose = newPrice,
    //             UtilisateurId = CurrentUser!.UtilisateurId
    //         };
    //     
    //         _messageService.PostMessageDemande(messageDemandePostDto);
    //     
    //         var conv = Conversations.FirstOrDefault(c => c.ConversationId == SelectedConversation.ConversationId);
    //         
    //         if (conv != null)
    //         {
    //             conv.LastMessage = "demande";
    //             conv.HasNewMessages = false;
    //
    //             try
    //             {
    //                 Conversations.Remove(conv);
    //                 Conversations.Insert(0, conv);
    //             }
    //             catch (Exception ex)
    //             {
    //                 Console.WriteLine($"[VM] Error moving conversation to top after sending: {ex.Message}");
    //             }
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"Erreur envoi message: {ex.Message}");
    //     }
    //     finally
    //     {
    //         NotifyStateChanged();
    //     }
    // }
    
    // À ajouter dans MessagerieViewModel.cs

    public async Task SendProposition(double proposedPrice)
    {
        if (SelectedConversation == null || CurrentUser == null)
            return;

        try
        {
            var demande = new MessageDemandePostDTO
            {
                ConversationId = SelectedConversation.ConversationId,
                UtilisateurId = CurrentUser.UtilisateurId,
                PrixPropose = proposedPrice,
            };

            await _messageService.PostMessageDemande(demande);
            
            // Mettre à jour la conversation dans la liste
            var conv = Conversations.FirstOrDefault(c => c.ConversationId == SelectedConversation.ConversationId);
            if (conv != null)
            {
                conv.LastMessage = $"Proposition: {proposedPrice} €";
                conv.HasNewMessages = false;
            }

            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[VM] ❌ Error sending price proposal: {ex.Message}");
            throw;
        }
    }

    public async Task AnswerPriceProposal(int messageId, bool accepted)
    {
        if (SelectedConversation == null)
            return;

        try
        {
            // Appeler l'API pour accepter la proposition
            await _messageService.AnswerPriceProposal(messageId, accepted);

            // Mettre à jour localement le message
            var message = SelectedConversation.ListMessages?
                .OfType<MessageDemandeDTO>()
                .FirstOrDefault(m => m.MessageId == messageId);

            if (message != null)
            {
                message.EstAcceptee = accepted;
                message.EstRepondue = true;
            }

            // Notifier via SignalR
            await _signalRService.NotifyProposalResponse(
                SelectedConversation.ConversationId, 
                messageId, 
                accepted
            );

            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[VM] ❌ Error accepting proposal: {ex.Message}");
            throw;
        }
    }

    // public async Task DeclinePriceProposal(int messageId)
    // {
    //     if (SelectedConversation == null)
    //         return;
    //
    //     try
    //     {
    //         // Appeler l'API pour refuser la proposition
    //         await _messageService.DeclinePriceProposal(messageId);
    //
    //         // Mettre à jour localement le message
    //         var message = SelectedConversation.ListMessages?
    //             .OfType<MessageDemandeDTO>()
    //             .FirstOrDefault(m => m.MessageId == messageId);
    //
    //         if (message != null)
    //         {
    //             message.EstAcceptee = false;
    //             message.EstRepondue = true;
    //         }
    //
    //         // Notifier via SignalR
    //         await _signalRService.NotifyProposalResponse(
    //             SelectedConversation.ConversationId, 
    //             messageId, 
    //             false
    //         );
    //
    //         NotifyStateChanged();
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine($"[VM] ❌ Error declining proposal: {ex.Message}");
    //         throw;
    //     }
    // }

    private async void HandleMessageReceived(int conversationId, int senderId, string message, List<int> photoIds, DateTime date)
    {
        if (SelectedConversation != null && SelectedConversation.ConversationId == conversationId)
        {
            var exists = SelectedConversation.ListMessages?.Any(m =>
                m.SenderId == senderId &&
                //m.Content == message &&
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
                    //ConversationId = conversationId,
                    SentByCurrentUser = senderId == CurrentUser?.UtilisateurId,
                    Photos = photoIds,
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
                        await _messageService.MaskAsRead(newMessage.MessageId.Value);
                        
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
                    .Where(m => m.SenderId == CurrentUser!.UtilisateurId && m.SentByCurrentUser == true && m.Lu == false)
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
    
    private void HandleProposalResponse(int conversationId, int messageId, bool accepted)
    {
        if (SelectedConversation?.ConversationId == conversationId)
        {
            var message = SelectedConversation.ListMessages?
                .OfType<MessageDemandeDTO>()
                .FirstOrDefault(m => m.MessageId == messageId);

            if (message != null)
            {
                message.EstAcceptee = accepted;
                message.EstRepondue = true;
                NotifyStateChanged();
            }
        }
    }

    public void Dispose()
    {
        _signalRService.OnMessageReceived -= HandleMessageReceived;
        _signalRService.OnUserTyping -= HandleUserTyping;
        _signalRService.OnMessagesRead -= HandleMessagesRead;
        _signalRService.OnProposalResponse -= HandleProposalResponse; 
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
    
    public async Task OnImagesSelectedAsync(InputFileChangeEventArgs e)
    {
        SelectedFilePreviews.Clear();
        SelectedFile = e.GetMultipleFiles().ToList();

        foreach (var file in SelectedFile)
        {
            using var ms = new MemoryStream();
            await file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024).CopyToAsync(ms);

            var base64 = $"data:{file.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";
            SelectedFilePreviews.Add((file, base64));
        }

        NotifyStateChanged();
    }
    
    public void RemoveSelectedPhotoAt(int index)
    {
        if (index < 0 || index >= SelectedFilePreviews.Count)
            return;

        SelectedFilePreviews.RemoveAt(index);

        if (SelectedFile != null && index < SelectedFile.Count)
            SelectedFile.RemoveAt(index);

        NotifyStateChanged();
    }

    
    
}
