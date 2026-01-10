using Shared.DTO;
using Shared.DTO.Annonce;
using Shared.DTO.Favoris;
using Shared.DTO.Conversation;
using Shared.DTO.Photo;
using Shared.DTO.Signalement;
using Shared.DTO.Utilisateur;
using FrontBlazor.Services;
using FrontBlazor.Services.Interfaces;
using FrontBlazor.ViewModel.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Shared.DTO.Recense;

namespace FrontBlazor.ViewModel;

public class DetailAnnonceViewModel : ClientBaseViewModel
{
    private readonly IAnnonceService _annonceService;
    private readonly IFavorisService<FavorisDTO> _favorisService;
    private readonly IAuthService _authService;
    private readonly IConversationService<ConversationDTO> _conversationService;
    private readonly IUtilisateurService _utilisateurService;
    private readonly NavigationManager _navigationManager;
    private readonly ClipboardService _clipboardService;
    private readonly IMediasService _mediaService;
    private readonly IVisualisationService _visualisationService;
    private readonly ISignalementService _signalementService;
    private readonly IRecenseService<RecenseDetailDTO> _recenseWebService;

    private CancellationTokenSource? _viewTimerCts;

    public AnnonceDetailDTO? AnnonceDetail { get; set; }
    public UtilisateurViewDTO? utilisateurAnnonce { get; set; }
    public List<AnnonceDTO>? similarAnnonces = null;
    public bool IsLoading { get; set; }
    public string? ErrorMessage { get; set; }
    public bool clickedShareButton { get; set; } = false;

    public bool IsSameUser { get; set; } = true;

    public bool IsUserSuspended { get; set; } = false;
    public bool IsBlockedByUser { get; set; } = false;
    public bool ShowSignalerModal { get; set; } = false;
    public string SignalementRaison { get; set; } = string.Empty;
    public bool IsSubmittingReport { get; set; } = false;
    public bool IsLoadingSimilar { get; set; }

    public DetailAnnonceViewModel(IAnnonceService annonceService,
        IFavorisService<FavorisDTO> favorisService, IAuthService authService,
        IUtilisateurService utilisateurService,
        IConversationService<ConversationDTO> conversationService,
        ClipboardService clipboardService, NavigationManager navigationManager, 
        IMediasService mediasService,
        IVisualisationService visualisationService, 
        ISignalementService signalementService,
        INotificationService notificationService,
        IRecenseService<RecenseDetailDTO> recenseWebService
        )
        : base(navigationManager, authService, notificationService)
    
    {
        _annonceService = annonceService;
        _favorisService = favorisService;
        _authService = authService;
        _utilisateurService = utilisateurService;
        _conversationService = conversationService;
        _mediaService = mediasService;
        _navigationManager = navigationManager;
        _clipboardService = clipboardService;
        _visualisationService = visualisationService;
        _signalementService = signalementService;
        _recenseWebService = recenseWebService;

    }

    public async Task LoadAnnonceDetailAsync(int id)
    {
        if (AnnonceDetail != null && AnnonceDetail.AnnonceId == id)
            return;

        IsLoading = true;
        ErrorMessage = null;
        IsLoadingSimilar = true;
        PageNumber = 1;
        await base.LoadAsync();
        try
        {
            AnnonceDetail = await _annonceService.GetAnnonceDetailById(id);
            if (AnnonceDetail == null)
            {
                ErrorMessage = "Annonce introuvable";
                return;
            }
            else
            {
                await StartVisualisationTimer(AnnonceDetail.AnnonceId);
            }

            utilisateurAnnonce = await _utilisateurService.GetUserById(AnnonceDetail.UtilisateurId);
            IsBlockedByUser = utilisateurAnnonce.BlockedByCurrentUser;
            if (utilisateurAnnonce == null || utilisateurAnnonce.Statut == "Suspendu")
            {
                IsUserSuspended = true;
                return;
            }

            IsSameUser = await CheckIfOwnerAnnonce(id, "AnnonceDetail");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Erreur lors du chargement de l'annonce";
            Console.WriteLine($"Error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }

        similarAnnonces = await _annonceService.GetSimilarAnnonces(id, PageNumber, 4);
        IsLoadingSimilar = false;
    }
    public async Task ToggleFavorite(dynamic annonce)
    {
        if (utilisateur == null)
        {
            _navigationManager.NavigateTo("/login");
            return;
        }

        bool isFavorite = annonce.IsLikedByCurrentUser;
        annonce.IsLikedByCurrentUser = !annonce.IsLikedByCurrentUser;

        try
        {
            if (!isFavorite)
            {
                await _favorisService.AddFavoris(annonce.AnnonceId);
                annonce.NombreLikes += 1;
            }
            else
            {
                await _favorisService.DeleteFavoris(annonce.AnnonceId);
                annonce.NombreLikes -= 1;
            }
        }
        catch
        {
            annonce.IsLikedByCurrentUser = isFavorite;
        }
    }
    public void GoBack()
    {
        _navigationManager.NavigateTo("/search");
    }

    public async void ContactSeller()
    {
        if (utilisateur == null)
        {
            _navigationManager.NavigateTo("/login");
            return;
        }

        // check if conversation already exists, if not then create and send to page
        var conv = await _conversationService.GetOrCreateConversation(AnnonceDetail.AnnonceId);
        _navigationManager.NavigateTo($"/messages?conversationId={conv.ConversationId}");
    }

    public async void MakeOffer()
    {
        if (utilisateur == null)
        {
            _navigationManager.NavigateTo("/login");
            return;
        }

        var conv = await _conversationService.GetOrCreateConversation(AnnonceDetail.AnnonceId);
        _navigationManager.NavigateTo($"/messages?conversationId={conv.ConversationId}&makeOffer=true");
    }

    public async void BuyProduct()
    {
        if (utilisateur == null)
        {
            _navigationManager.NavigateTo("/login");
            return;
        }
        var conv = await _conversationService.GetOrCreateConversation(AnnonceDetail.AnnonceId);
        _navigationManager.NavigateTo($"/acheter/{conv.ConversationId}");
    }

    public async void ShareProduct()
    {
        clickedShareButton = false;
        string url = _navigationManager.Uri.ToString();
        _clipboardService.Copy(url);
        clickedShareButton = true;
    }

    public void NavigateToProduct(int productId)
    {
        _navigationManager.NavigateTo($"/product/{productId}", forceLoad: true);
    }

    public void GotoProfile()
    {
        _navigationManager.NavigateTo($"/profile/" + utilisateurAnnonce.Login);
    }

    public string GetPhoto(int id)
    {
        return _mediaService.GetPhotoUrl(id);
    }

    private async Task StartVisualisationTimer(int annonceId)
    {
        _viewTimerCts?.Cancel();
        _viewTimerCts = new CancellationTokenSource();

        try
        {
            await Task.Delay(TimeSpan.FromSeconds(5), _viewTimerCts.Token);
            await _visualisationService.CreateVisualisationAsync(annonceId);
        }
        catch (TaskCanceledException)
        {
            // L'utilisateur a quitté la page avant 5 secondes
        }
    }

    public void CancelVisualisation()
    {
        _viewTimerCts?.Cancel();
    }

    public void NavigateToHome()
    {
        _navigationManager.NavigateTo("/");
    }

    public void ToggleSignalerModal()
    {
        if (utilisateur == null)
        {
            _navigationManager.NavigateTo("/login");
            return;
        }
        ShowSignalerModal = !ShowSignalerModal;
    }
    public async Task SubmitReport()
    {
        IsSubmittingReport = true;
        try
        {
            SignalementAnnonceCreateDTO newReport = new SignalementAnnonceCreateDTO
            {
                SignalementMotif = SignalementRaison,
                AnnonceSignaleeId = AnnonceDetail.AnnonceId,
            };

            SignalementDetailsDTO reuslt = await _signalementService.CreateSignalement(newReport);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la soumission du signalement : {ex.Message}");
        }
        finally
        {
            IsSubmittingReport = false;
            ShowSignalerModal = false;

        }
    }
    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();
    
    public int PageNumber { get; set; }
    public async Task PreviousSimilar()
    {
        if (PageNumber > 1)
        {
            PageNumber--;
            similarAnnonces = await _annonceService.GetSimilarAnnonces(AnnonceDetail.AnnonceId,PageNumber,4);
        }
        NotifyStateChanged();
    }
    public async Task NextSimilar()
    {
        if (similarAnnonces.Count == 4)
        {
            List<AnnonceDTO> newAnnonces = await _annonceService.GetSimilarAnnonces(AnnonceDetail.AnnonceId,PageNumber + 1,4 );
            if (newAnnonces.Count > 0)
            {
                PageNumber++;
                similarAnnonces = newAnnonces;
                NotifyStateChanged();
            }
        }
    }

   public async Task<bool> CheckIfOwnerAnnonce(int annonceId, string typeAnnonce)
    {
        CurrentUtilisateurDTO utilisateur = await _authService.GetCurrentUserAsync();
        if (utilisateur == null)
            return false;

        switch (typeAnnonce?.ToLower())
        {
            case "announcedetail":
                return AnnonceDetail != null && AnnonceDetail.UtilisateurId == utilisateur.UtilisateurId;

            case "similarannonce":
                var annonce = similarAnnonces?.FirstOrDefault(a => a.AnnonceId == annonceId);
                return annonce != null && annonce.IdAuteur == utilisateur.UtilisateurId;

            default:
                return false;
        }
    }
}
