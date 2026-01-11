using System.Collections.ObjectModel;
using FrontBlazor.Services;
using Shared.DTO;
using Shared.DTO.Conversation;
using Shared.DTO.Message;
using Shared.DTO.Utilisateur;
using FrontBlazor.Services.Interfaces;
using FrontBlazor.Services.WebService;
using FrontBlazor.ViewModel.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Shared.DTO.Photo;
using Shared.DTO.Signalement;

namespace FrontBlazor.ViewModel;

public class MessagerieViewModel : ClientBaseViewModel, IDisposable
{
    private readonly IConversationService<ConversationDTO> _conversationService;
    private readonly IAuthService _authService;
    public readonly IMediasService _mediaService;
    private readonly IMessageService _messageService;
    public readonly ISignalRService _signalRService;
    private readonly ISignalementService _signalementService;
    private readonly IUtilisateurService _utilisateurService;
    private readonly SignalRHandlerWebService _signalRHandler;
    private readonly NavigationManager _nav;

    public ObservableCollection<ConversationDTO> Conversations { get; private set; } = new();
    public ConversationDTO? SelectedConversation { get; private set; }
    public int? SelectedConversationId { get; private set; }
    //public UtilisateurDTO? CurrentUser { get; private set; }
    public IBrowserFile? ColisPhoto { get; private set; }
    public string? ColisPhotoPreviewBase64 { get; private set; }

    public bool IsSendingColis { get; private set; }
    public string? ColisError { get; private set; }

    public string NewMessage { get; set; } = "";
    public List<IBrowserFile> SelectedFile { get; set; }
    public List<(IBrowserFile File, string PreviewBase64)> SelectedFilePreviews { get; set; } = new();
    public bool IsLoading { get; private set; } = false;
    public string? PriceProposalError { get; set; }

    public ElementReference MessagesContainer;
    public event Action? OnMessageReceivedUI; 

    private System.Threading.Timer? _typingTimer;
    private System.Threading.Timer? _typingDisplayTimer;
    private bool _typingNotified = false;
    
    public bool IsTyping => _signalRHandler.IsTyping;
    public string TypingUserName => _signalRHandler.TypingUserName;

    public List<string> ErrorMessages { get; private set; } = new();
    public bool ShowReportModal { get; set; }
    public bool IsSubmittingReport { get; set; }
    private int? ReportingMessageId { get; set; }
    public string SignalementRaison { get; set; } = string.Empty;
    public string ReportingMessageContent { get; set; } = string.Empty;
    
    public bool ShowReceptionModal { get; set; }
    public bool ColisEstConforme { get; set; }
    public IBrowserFile? ReceptionPhoto { get; set; }
    public string? ReceptionPhotoPreviewBase64 { get; set; }
    public string ReceptionDescription { get; set; } = "";
    public bool IsSendingReception { get; set; }
    public string? ReceptionError { get; set; }
    public int? CurrentMessageEnvoieColisId { get; set; }
    public bool ColisDejaRecu { get; private set; }

    public MessagerieViewModel(
        IConversationService<ConversationDTO> conversationService,
        IUtilisateurService utilisateurService,
        IAuthService authService,
        IMessageService messageService,
        IMediasService mediaService,
        NavigationManager nav,
        ISignalementService signalementService,
        NavigationManager navigationManager,
        ISignalRService signalRService,
        SignalRHandlerWebService signalRHandlerWebService,
        INotificationService notificationService)
        : base(navigationManager, authService,signalRService, notificationService)
    
    {
        _conversationService = conversationService;
        _authService = authService;
        _nav = nav;
        _messageService = messageService;
        _signalRService = signalRService;
        _utilisateurService = utilisateurService;
        _mediaService = mediaService;
        _signalRHandler = signalRHandlerWebService;
        _signalementService = signalementService;
        
        _signalRHandler.OnStateChanged += NotifyStateChanged;
        _signalRHandler.OnMessageReceivedUI += () => OnMessageReceivedUI?.Invoke();
        
        // _signalRHandler.OnMessageReceived += HandleMessageReceived;
        // _signalRHandler.OnUserTyping += HandleUserTyping;
        // _signalRHandler.OnMessagesRead += HandleMessagesRead;
        // _signalRHandler.OnProposalResponse += HandleProposalResponse;
        // _signalRHandler.OnPriceProposalReceived += HandlePriceProposalReceived;
    }

    public override async Task LoadAsync()
    {
        await base.LoadAsync();
        if (utilisateur == null)
        {
            _nav.NavigateTo("/");
        }
    }

    public async Task LoadConversationsAsync()
    {
        IsLoading = true;
        NotifyStateChanged();

        if (utilisateur == null)
        {
            IsLoading = false;
            NotifyStateChanged();
            return;
        }

        var data = await _conversationService.GetConversationsByUserId(utilisateur.UtilisateurId);
        
        if (data != null)
        {
            var validConversations = data.Where(c => 
                !string.IsNullOrEmpty(c.TitreAnnonce) && 
                !string.IsNullOrEmpty(c.Interlocuteur)
            ).ToList();
        
            Conversations = new ObservableCollection<ConversationDTO>(validConversations);
        }
        else
        {
            Conversations = new ObservableCollection<ConversationDTO>();
        }
        await _signalRService.StartAsync();
        
        foreach (var c in Conversations)
        {
            await _signalRService.JoinConversation(c.ConversationId);
            if (c.ListMessages.LastOrDefault()?.SentByCurrentUser == false && c.ListMessages.LastOrDefault().Lu == false)
            {
                c.HasNewMessages = true;
            }
        }

        if (utilisateur != null)
        {
            _signalRHandler.SetContext(
                SelectedConversation,
                Conversations,
                SelectedConversationId,
                utilisateur.UtilisateurId
            );
        }
        
        IsLoading = false;
        NotifyStateChanged();
    }

    public async Task SelectConversationAsync(int conversationId)
    {
        if (utilisateur == null)
            utilisateur = await _authService.GetCurrentUserAsync();

        SelectedConversationId = conversationId;
        
        var conv = await _conversationService.GetConversationDetailById(conversationId);

        if (conv != null)
        {
            if (string.IsNullOrEmpty(conv.TitreAnnonce))
            {
                conv.TitreAnnonce = "Annonce supprimée";
            }
            
            if (string.IsNullOrEmpty(conv.Interlocuteur))
            {
                conv.Interlocuteur = "Utilisateur supprimé";
            }
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
        NewsDTO updateNotifs = await _utilisateurService.GetActivity();
    
        // ✅ Mettre à jour localement
        NotificationCount = updateNotifs.NotificationsCount;
        MessageCount = updateNotifs.MessagesCount;
        
        await base.LoadAsync(); 
        
        if (utilisateur != null)
        {
            _signalRHandler.SetContext(
                SelectedConversation,
                Conversations,
                SelectedConversationId,
                utilisateur.UtilisateurId
            );
        }
    
        await _signalRService.MarkMessagesAsRead(conversationId, utilisateur!.UtilisateurId);
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
                UtilisateurId = utilisateur!.UtilisateurId,
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
                
            if (utilisateur != null)
            {
                _signalRHandler.SetContext(
                    SelectedConversation,
                    Conversations,
                    SelectedConversationId,
                    utilisateur.UtilisateurId
                );
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

    public async Task SendProposition(decimal proposedPrice)
    {
        if (SelectedConversation == null || utilisateur == null)
            return;
        
        PriceProposalError = null;

        if (proposedPrice > SelectedConversation.PrixAnnonce)
        {
            PriceProposalError = "Le prix proposé ne peut pas être supérieur au prix.";
            NotifyStateChanged();
            return;
        }

        // ❌ Prix inférieur à 50% du prix de l'annonce
        if (proposedPrice < SelectedConversation.PrixAnnonce * 0.5m)
        {
            PriceProposalError = "Le prix proposé ne peut pas être inférieur à 50% du prix initial.";
            NotifyStateChanged();
            return;
        }

        try
        {
            var demande = new MessageDemandePostDTO
            {
                ConversationId = SelectedConversation.ConversationId,
                UtilisateurId = utilisateur.UtilisateurId,
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
            
            if (utilisateur != null)
            {
                _signalRHandler.SetContext(
                    SelectedConversation,
                    Conversations,
                    SelectedConversationId,
                    utilisateur.UtilisateurId
                );
            }

            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[VM] ❌ Error sending price proposal: {ex.Message}");
            throw;
        }
    }
    
    public async Task OnColisPhotoSelected(InputFileChangeEventArgs e)
    {
        ColisError = null;
        ColisPhoto = null;
        ColisPhotoPreviewBase64 = null;

        var file = e.File;

        if (!file.ContentType.StartsWith("image/"))
        {
            ColisError = "Le fichier doit être une image";
            NotifyStateChanged();
            return;
        }
        
        if (file.Size > 10 * 1024 * 1024)
        {
            ColisError = "La photo ne doit pas dépasser 10 Mo";
            NotifyStateChanged();
            return;
        }

        using var ms = new MemoryStream();
        await file.OpenReadStream(10 * 1024 * 1024).CopyToAsync(ms);

        ColisPhoto = file;
        ColisPhotoPreviewBase64 =
            $"data:{file.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";

        NotifyStateChanged();
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
            
            if (utilisateur != null)
            {
                _signalRHandler.SetContext(
                    SelectedConversation,
                    Conversations,
                    SelectedConversationId,
                    utilisateur.UtilisateurId
                );
            }

            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[VM] ❌ Error accepting proposal: {ex.Message}");
            throw;
        }
    }
    
    public void HandleTyping(KeyboardEventArgs e)
    {
        if (SelectedConversation == null || utilisateur == null)
            return;

        _typingTimer?.Dispose();
        _typingTimer = new System.Threading.Timer(_ =>
        {
            _typingNotified = false;
        }, null, 2000, Timeout.Infinite);

        if (_typingNotified)
            return;

        _typingNotified = true;
        
        _ = _signalRService.NotifyTyping(SelectedConversation.ConversationId, utilisateur.UtilisateurId, utilisateur.Login ?? "Utilisateur");
    }

    public void Dispose()
    {
        _signalRHandler.OnStateChanged -= NotifyStateChanged;
        _signalRHandler.Dispose();
        _typingTimer?.Dispose();
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
        const int maxFileSize = 10 * 1024 * 1024;
        const int maxPhotos = 5;

        if (e.GetMultipleFiles().Count > maxPhotos)
        {
            AddError($"Maximum {maxPhotos} photos autorisées");
            return;
        }
        
        // SelectedFilePreviews.Clear();
        SelectedFile = e.GetMultipleFiles().ToList();

        foreach (var file in SelectedFile)
        {
            try
            {
                if (file.Size > maxFileSize)
                {
                    AddError($"{file.Name} est trop volumineux (max 10MB)");
                    continue;
                }

                if (!file.ContentType.StartsWith("image/"))
                {
                    AddError($"{file.Name} n'est pas une image valide");
                    continue;
                }

                using var ms = new MemoryStream();
                await file.OpenReadStream(maxAllowedSize: maxFileSize).CopyToAsync(ms);

                var base64 = Convert.ToBase64String(ms.ToArray());
                var dataUrl = $"data:{file.ContentType};base64,{base64}";

                SelectedFilePreviews.Add((file, dataUrl));

                Console.WriteLine($"✅ Photo ajoutée: {file.Name} ({file.Size} bytes)");
            }
            catch (Exception ex)
            {
                AddError($"Erreur lors du chargement de {file.Name}: {ex.Message}");
                Console.WriteLine($"❌ Erreur photo upload: {ex.Message}");
            }
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
    public void OpenReportModal(int messageId)
    {
        var message = SelectedConversation?.ListMessages?
            .FirstOrDefault(m => m.MessageId == messageId);

        if (message == null)
        {
            Console.WriteLine("⚠️ Message introuvable dans la conversation.");
            return;
        }

        ReportingMessageId = messageId;
        SignalementRaison = string.Empty;
        ReportingMessageContent = (message as MessageTextDTO)?.Content ?? string.Empty;

        ShowReportModal = true;
        NotifyStateChanged();
    }

    public void CloseReportModal()
    {
        ShowReportModal = false;
        ReportingMessageId = null;
        ReportingMessageContent = string.Empty;
        SignalementRaison = string.Empty;
        NotifyStateChanged();
    }
    public async Task ConfirmReportMessageAsync()
    {
        if (!ReportingMessageId.HasValue || string.IsNullOrWhiteSpace(SignalementRaison))
        {
            Console.WriteLine("⚠️ Veuillez fournir une raison pour le signalement");
            return;
        }

        if (IsSubmittingReport) return;

        IsSubmittingReport = true;
        NotifyStateChanged();

        if (ReportingMessageId == null)
        {
            Console.WriteLine("⚠️ Aucun message sélectionné pour le signalement.");
            return;
        }

        if (SelectedConversation == null)
        {
            Console.WriteLine("⚠️ Pas de conversation sélectionnée.");
            return;
        }

        if (_signalementService == null)
        {
            Console.WriteLine("⚠️ Service de signalement non initialisé.");
            return;
        }


        try
        {
            var dto = new SignalementMessageCreateDTO
            {
                MessageId = ReportingMessageId.Value,
                SignalementMotif = SignalementRaison
            };

            var result = await _signalementService.CreateSignalement(dto);

            if (result != null)
            {
                CloseReportModal(); // safely closes and resets UI
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur lors du signalement: {ex.Message}");
        }
        finally
        {
            IsSubmittingReport = false;
            NotifyStateChanged();
        }
    }
    
    private void AddError(string errorMessage)
    {
        ErrorMessages.Add(errorMessage);
        NotifyStateChanged();
    }

    public void ClearErrors()
    {
        ErrorMessages.Clear();
        NotifyStateChanged();
    }
    
    public async Task MarquerColisEnvoyeAsync(int messagePayeeId)
    {
        if (SelectedConversation == null || utilisateur == null)
            return;

        if (ColisPhoto == null)
        {
            ColisError = "Une photo est obligatoire pour prouver l’envoi du colis";
            NotifyStateChanged();
            return;
        }

        IsSendingColis = true;
        ColisError = null;
        NotifyStateChanged();

        try
        {
            var bytes = await ConvertIBrowserFileToBytesAsync(ColisPhoto);

            var photoDto = new PhotoUploadDTO
            {
                FileName = ColisPhoto.Name,
                ContentType = ColisPhoto.ContentType,
                FileSize = ColisPhoto.Size,
                Base64Data = Convert.ToBase64String(bytes)
            };

            var dto = new MessageEnvoisColisPostDTO
            {
                ConversationId = SelectedConversation.ConversationId,
                UtilisateurId = utilisateur.UtilisateurId,
                Photo = photoDto,
                MessageEstPayeeId = messagePayeeId
            };

            await _messageService.PostMessageEnvoieColis(dto);

            // Reset UI
            ColisPhoto = null;
            ColisPhotoPreviewBase64 = null;
        }
        catch (Exception ex)
        {
            ColisError = "Erreur lors de l’envoi du colis";
            Console.WriteLine(ex.Message);
        }
        finally
        {
            IsSendingColis = false;
            NotifyStateChanged();
        }
    }

    public async Task CancelMessagePayeeAsync(int id)
    {
        await _messageService.CancelMessagePayee(id);
    }
    
    public void OuvrirModalReception(bool estConforme, int messageEnvoieId)
{
    ColisEstConforme = estConforme;
    ShowReceptionModal = true;
    ReceptionDescription = "";
    ReceptionPhoto = null;
    ReceptionPhotoPreviewBase64 = null;
    ReceptionError = null;
    
    // Trouver le MessageEnvoieColisId
    var colisMessage = SelectedConversation?.ListMessages?
        .OfType<MessageEnvoieColisDTO>()
        .LastOrDefault();
    
    if (colisMessage != null)
    {
        CurrentMessageEnvoieColisId = messageEnvoieId;
    }
    
    NotifyStateChanged();
}

    public void FermerModalReception()
    {
        ShowReceptionModal = false;
        ColisEstConforme = false;
        ReceptionDescription = "";
        ReceptionPhoto = null;
        ReceptionPhotoPreviewBase64 = null;
        ReceptionError = null;
        CurrentMessageEnvoieColisId = null;
        NotifyStateChanged();
    }

    public async Task OnReceptionPhotoSelected(InputFileChangeEventArgs e)
    {
    ReceptionError = null;
    ReceptionPhoto = null;
    ReceptionPhotoPreviewBase64 = null;

    var file = e.File;

    if (!file.ContentType.StartsWith("image/"))
    {
        ReceptionError = "Le fichier doit être une image";
        NotifyStateChanged();
        return;
    }
    
    if (file.Size > 10 * 1024 * 1024)
    {
        ReceptionError = "La photo ne doit pas dépasser 10 Mo";
        NotifyStateChanged();
        return;
    }

    using var ms = new MemoryStream();
    await file.OpenReadStream(10 * 1024 * 1024).CopyToAsync(ms);

    ReceptionPhoto = file;
    ReceptionPhotoPreviewBase64 = $"data:{file.ContentType};base64,{Convert.ToBase64String(ms.ToArray())}";

    NotifyStateChanged();
}

    public async Task ConfirmerReceptionColisAsync()
    {
    if (SelectedConversation == null || utilisateur == null || CurrentMessageEnvoieColisId == null)
        return;

    // Validation : si non conforme, photo et description obligatoires
    if (!ColisEstConforme)
    {
        if (ReceptionPhoto == null)
        {
            ReceptionError = "Une photo est obligatoire pour signaler un colis non conforme";
            NotifyStateChanged();
            return;
        }
        
        if (string.IsNullOrWhiteSpace(ReceptionDescription))
        {
            ReceptionError = "Une description est obligatoire pour signaler un colis non conforme";
            NotifyStateChanged();
            return;
        }
    }

    IsSendingReception = true;
    ReceptionError = null;
    NotifyStateChanged();

    try
    {
        PhotoUploadDTO? photoDto = null;
        
        if (ReceptionPhoto != null)
        {
            var bytes = await ConvertIBrowserFileToBytesAsync(ReceptionPhoto);
            photoDto = new PhotoUploadDTO
            {
                FileName = ReceptionPhoto.Name,
                ContentType = ReceptionPhoto.ContentType,
                FileSize = ReceptionPhoto.Size,
                Base64Data = Convert.ToBase64String(bytes)
            };
        }

        var dto = new MessageEstRecuPostDTO
        {
            ConversationId = SelectedConversation.ConversationId,
            UtilisateurId = utilisateur.UtilisateurId,
            EstConforme = ColisEstConforme,
            Photo = photoDto,
            Description = string.IsNullOrWhiteSpace(ReceptionDescription) ? null : ReceptionDescription,
            MessageEstEnvoieId = CurrentMessageEnvoieColisId.Value
        };

        await _messageService.PostMessageEstRecu(dto);

        // Marquer comme reçu
        ColisDejaRecu = true;
        
        FermerModalReception();
    }
    catch (Exception ex)
    {
        ReceptionError = "Erreur lors de la confirmation de réception";
        Console.WriteLine($"[VM] ❌ Error confirming reception: {ex.Message}");
    }
    finally
    {
        IsSendingReception = false;
        NotifyStateChanged();
    }
}
}