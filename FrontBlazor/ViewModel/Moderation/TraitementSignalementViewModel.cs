using FrontBlazor.Pages.Moderation;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Shared.DTO;
using Shared.DTO.Annonce;
using Shared.DTO.Moderation;
using Shared.DTO.NoteUtilisateur;
using Shared.DTO.Signalement;
using Shared.DTO.Utilisateur;
using System.ComponentModel;
using Shared.DTO.Conversation;
using Shared.DTO.Decision;
using Shared.DTO.Message;
using Shared.DTO.Photo;

namespace FrontBlazor.ViewModel.Moderation.Signalements;

public class TraitementSignalementViewModel : ModerationViewModel, INotifyPropertyChanged
{
    private readonly ISignalementService _signalementService;
    private readonly IUtilisateurService _utilisateurService;
    private readonly IDecisionService _decisionService;
    private readonly IAnnonceService _annonceService;
    private readonly INoteUtilisateurService _noteUtilisateurService;
    private readonly IConversationService<ConversationDTO> _conversationService;
    private readonly INotificationService _notificationService;
    private readonly NavigationManager _nav;
    private readonly IMediasService _mediasService;
    public event PropertyChangedEventHandler PropertyChanged;
    public event Action OnStateChanged;

    public TraitementSignalementViewModel(
        ISignalementService signalementService,
        IUtilisateurService utilisateurService,
        IDecisionService decisionService,
        IConversationService<ConversationDTO> conversationService,
        IAnnonceService annonceService,
        IMediasService mediasService,
        INoteUtilisateurService noteUtilisateurService,
        INotificationService notificationService,
        IAuthService authService,
        NavigationManager nav)
        : base(authService, nav)
    {
        _utilisateurService = utilisateurService;
        _notificationService = notificationService;
        _annonceService = annonceService;
        _conversationService = conversationService;
        _decisionService =  decisionService;
        _noteUtilisateurService = noteUtilisateurService;
        _signalementService = signalementService;
        _mediasService = mediasService;
        _nav = nav;
    }

    public SignalementDetailsDTO Signalement { get; set; }
    public int SuspendDays { get; set; }
    public string? PhotoProfilUrl { get; set; }
    public List<string> PhotosUrl { get; set; } = new();
    public NoteUtilisateurDetailDTO Avis { get; set; }
    public AnnonceDetailDTO Annonce { get; set; }
    public MessageSignalementDTO Message { get; set; }
    public UtilisateurViewDTO UtilisateurSignale { get; set; }
    
    private bool _showWarningModal;
    public bool ShowWarningModal
    {
        get => _showWarningModal;
        set
        {
            _showWarningModal = value;
            NotifyStateChanged();
        }
    }

    private bool _showSuspendModal;
    public bool ShowSuspendModal
    {
        get => _showSuspendModal;
        set
        {
            _showSuspendModal = value;
            NotifyStateChanged();
        }
    }

    private bool _showBanModal;
    public bool ShowBanModal
    {
        get => _showBanModal;
        set
        {
            _showBanModal = value;
            NotifyStateChanged();
        }
    }

    private string _warningMessage = string.Empty;
    public string WarningMessage
    {
        get => _warningMessage;
        set
        {
            _warningMessage = value;
            NotifyStateChanged();
        }
    }

    private string _suspendMessage = string.Empty;
    public string SuspendMessage
    {
        get => _suspendMessage;
        set
        {
            _suspendMessage = value;
            NotifyStateChanged();
        }
    }

    public bool CanSendWarning => !string.IsNullOrWhiteSpace(WarningMessage);
    public bool CanSuspend => 
        !string.IsNullOrWhiteSpace(SuspendMessage) && 
        SuspendDays >= 1 && 
        SuspendDays <= 365;

    public async Task LoadSignalementAsync(int id)
    {
        await base.LoadAsync();
        Signalement = await _signalementService.GetSignalementByIdAsync(id);    
        switch (Signalement)
        {
            case SignalementAnnonceDTO sa:
                Annonce = await _annonceService.GetAnnonceDetailById(sa.AnnonceSignaleeId);
                PhotosUrl = await GetPhotosUrl(Annonce.Photos);
                break;
            case SignalementAvisDTO sav:
                Avis = await _noteUtilisateurService.GetByIdAsync(sav.AvisId);
                break;
            case SignalementMessageDTO sm:
                Message = await _conversationService.GetMessageById(sm.MessageId);
                PhotosUrl = await GetPhotosUrl(Message.Photos);
                break;
        }
        UtilisateurSignale = await _utilisateurService.GetUserById(Signalement.UtilisateurSignaleId);
        PhotoProfilUrl = await GetPhotoProfilUrl(); 
        NotifyStateChanged();
    }

    public void OpenWarningModal()
    {
        WarningMessage = string.Empty;
        ShowWarningModal = true;
    }

    public void CloseWarningModal()
    {
        ShowWarningModal = false;
        WarningMessage = string.Empty;
    }
    public void OpenSuspendModal()
    {
        SuspendMessage = string.Empty;
        SuspendDays = 7;
        ShowSuspendModal = true;
    }

    public void CloseSuspendModal()
    {
        ShowSuspendModal = false;
        SuspendMessage = string.Empty;
        SuspendDays = 7;
    }
    public void OpenBanModal()
    {
        ShowBanModal = true;
    }

    public void CloseBanModal()
    {
        ShowBanModal = false;
    }
    private void NotifyStateChanged()
    {
        OnStateChanged?.Invoke();
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }
    public async Task SendWarningAsync()
    {
        if (!CanSendWarning) return;

        try
        {
            await _notificationService.CreateNotificationAvertissement(
                new CreateAvertissementRequestDTO()
                {
                    MessageAvertissement = WarningMessage,
                    UtilisateurId = UtilisateurSignale.UtilisateurId
                }
            );
            CloseWarningModal();
            _nav.NavigateTo("/moderation/signalements");
            
            
        }
        catch (Exception ex)
        {
            
        }
    }

    public async Task SuspendUserAsync()
    {
        if (!CanSuspend) return;

        try
        {   
            var elementDecision = CreateElementDecision();
        
            var sanction = new SanctionSuspensionPostDTO
            {
                UtlisateurId = UtilisateurSignale.UtilisateurId,
                DateFinSuspension = DateTime.UtcNow.AddDays(SuspendDays),
                ElementDecision = elementDecision
            };
            var response = await _decisionService.AddDecision(sanction);
            CloseSuspendModal();
            _nav.NavigateTo("/moderation/signalements");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la suspension : {ex.Message}");
        }
    }
    private ElementDecisionDTO CreateElementDecision()
    {
        return Signalement switch
        {
            SignalementAnnonceDTO sa => new ElementDecisionAnnonceDTO 
            { 
                AnnonceId = sa.AnnonceSignaleeId 
            },
        
            SignalementAvisDTO sav => new ElementAvisDTO 
            { 
                AvisId = sav.AvisId 
            },
        
            SignalementMessageDTO sm => new ElementDecisionMessageDTO 
            { 
                MessageId = sm.MessageId 
            },
        
            SignalementUtilisateurDTO su => new ElementUtilisateurDTO 
            { 
                UtilisateurId = UtilisateurSignale.UtilisateurId 
            },
        
            _ => throw new InvalidOperationException($"Type de signalement non géré : {Signalement.GetType().Name}")
        };
    }
    public async Task BanUserAsync()
    {
        try
        {
            _nav.NavigateTo("/moderation/signalements");
            CloseBanModal();
        }
        catch (Exception ex)
        {
           
        }
    }

    public async Task DismissReportAsync()
    {
        try
        {
            await _signalementService.DeleteAsync(Signalement.SignalementId);
            _nav.NavigateTo("/moderation/signalements");
        }
        catch (Exception ex)
        {
        }
    }

    private async Task<List<string>> GetPhotosUrl(List<int> photosId)
    {
        List<string> photoUrls = new List<string>();
        foreach (int photoId in photosId)
        {
            photoUrls.Add(_mediasService.GetPhotoUrl(photoId));
        }
        return photoUrls;
    }

    private async Task<string?> GetPhotoProfilUrl()
    {
        if (UtilisateurSignale.PhotoProfilId == null)
        {
            return null;
        }
        return _mediasService.GetPhotoUrl(UtilisateurSignale.PhotoProfilId);
    }
}