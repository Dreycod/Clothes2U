using FrontBlazor.Models;
using FrontBlazor.Models.Moderation;
using FrontBlazor.Pages.Moderation;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using System.ComponentModel;

namespace FrontBlazor.ViewModel.Moderation.Signalements;

public class TraitementSignalementViewModel : ModerationViewModel, INotifyPropertyChanged
{
    private readonly ISignalementService _signalementService;
    private readonly IUtilisateurService _utilisateurService;
    private readonly IAnnonceService _annonceService;
    private readonly INoteUtilisateurService<NoteUtilisateur> _noteUtilisateurService;
    private readonly INotificationService _notificationService;
    private readonly NavigationManager _nav;
    private readonly IMediasService<Photo> _mediasService;
    public event PropertyChangedEventHandler PropertyChanged;
    public event Action OnStateChanged;

    public TraitementSignalementViewModel(
        ISignalementService signalementService,
        IUtilisateurService utilisateurService,
        IAnnonceService annonceService,
        IMediasService<Photo> mediasService,
        INoteUtilisateurService<NoteUtilisateur> noteUtilisateurService,
        INotificationService notificationService,
        IAuthService authService,
        NavigationManager nav)
        : base(authService, nav)
    {
        _utilisateurService = utilisateurService;
        _notificationService = notificationService;
        _annonceService = annonceService;
        _noteUtilisateurService = noteUtilisateurService;
        _signalementService = signalementService;
        _mediasService = mediasService;
        _nav = nav;
    }

    public SignalementDetails Signalement { get; set; }
    public string? PhotoProfilUrl { get; set; }
    public List<string> PhotosUrl { get; set; } = new();
    public NoteUtilisateur Avis { get; set; }
    public AnnonceDetail Annonce { get; set; }
    public UtilisateurView UtilisateurSignale { get; set; }
    
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

    private DateTime? _suspendEndDate;
    public DateTime? SuspendEndDate
    {
        get => _suspendEndDate;
        set
        {
            _suspendEndDate = value;
            NotifyStateChanged();
        }
    }

    public bool CanSendWarning => !string.IsNullOrWhiteSpace(WarningMessage);
    public bool CanSuspend => !string.IsNullOrWhiteSpace(SuspendMessage) && SuspendEndDate.HasValue;

    public async Task LoadSignalementAsync(int id)
    {
        await base.LoadAsync();
        Signalement = await _signalementService.GetSignalementByIdAsync(id);
        
        switch (Signalement)
        {
            case SignalementAnnonce sa:
                Annonce = await _annonceService.GetAnnonceDetailById(sa.AnnonceSignaleeId);
                PhotosUrl = await GetPhotosUrl();
                break;

            case SignalementAvis sav:
                Avis = await _noteUtilisateurService.GetByIdAsync(sav.AvisId);
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
        SuspendEndDate = null;
        ShowSuspendModal = true;
    }

    public void CloseSuspendModal()
    {
        ShowSuspendModal = false;
        SuspendMessage = string.Empty;
        SuspendEndDate = null;
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
                new CreateAvertissementRequest()
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
            
            
            _nav.NavigateTo("/moderation/signalements");
            CloseSuspendModal();
        }
        catch (Exception ex)
        {
        }
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
            _nav.NavigateTo("/moderation/signalements");
        }
        catch (Exception ex)
        {
        }
    }

    private async Task<List<string>> GetPhotosUrl()
    {
        List<string> photoUrls = new List<string>();
        foreach (int photoId in Annonce.Photos)
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