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
    private readonly IReadableService<UtilisateurView> _utilisateurService;
    private readonly IAnnonceService<Annonce> _annonceService;
    private readonly INoteUtilisateurService<NoteUtilisateur> _noteUtilisateurService;
    private readonly INotificationService _notificationService;

    public event PropertyChangedEventHandler PropertyChanged;
    public event Action OnStateChanged;

    public TraitementSignalementViewModel(
        ISignalementService signalementService,
        IReadableService<UtilisateurView> utilisateurService,
        IAnnonceService<Annonce> annonceService,
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
    }

    public SignalementDetails Signalement { get; set; }
    public NoteUtilisateur Avis { get; set; }
    public Annonce Annonce { get; set; }
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
                break;

            case SignalementAvis sav:
                Avis = await _noteUtilisateurService.GetByIdAsync(sav.AvisId);
                break;
        }
        
        UtilisateurSignale = await _utilisateurService.GetByIdAsync(Signalement.UtilisateurSignaleId);
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

            Console.WriteLine("Avertissement envoyé avec succès");
            CloseWarningModal();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de l'envoi de l'avertissement : {ex.Message}");
        }
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

    public async Task SuspendUserAsync()
    {
        if (!CanSuspend) return;

        try
        {

            Console.WriteLine($"Utilisateur suspendu jusqu'au {SuspendEndDate:dd/MM/yyyy HH:mm}");
            CloseSuspendModal();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la suspension : {ex.Message}");
        }
    }

    public void OpenBanModal()
    {
        ShowBanModal = true;
    }

    public void CloseBanModal()
    {
        ShowBanModal = false;
    }

    public async Task BanUserAsync()
    {
        try
        {
            Console.WriteLine("Utilisateur banni définitivement");
            CloseBanModal();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors du bannissement : {ex.Message}");
        }
    }

    public async Task DismissReportAsync()
    {
        try
        {

            Console.WriteLine("Signalement ignoré");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de l'ignorance du signalement : {ex.Message}");
        }
    }

    private void NotifyStateChanged()
    {
        OnStateChanged?.Invoke();
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
    }
}