using FrontBlazor.Models;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FrontBlazor.ViewModel;

public class DetailAnnonceViewModel
{
    private readonly IAnnonceService _annonceService;
    private readonly IFavorisService<Favoris> _favorisService;
    private readonly IAuthService _authService;
    private readonly IConversationService<Conversation> _conversationService;
    private readonly IUtilisateurService _utilisateurService;
    private readonly NavigationManager _navigationManager;
    private readonly ClipboardService _clipboardService;
    private readonly IMediasService<Photo> _mediaService;
    private readonly IVisualisationService _visualisationService;
    private readonly ISignalementService _signalementService;

    private CancellationTokenSource? _viewTimerCts;

    public AnnonceDetail? AnnonceDetail { get; set; }
    public UtilisateurView? utilisateurAnnonce { get; set; }
    public List<Annonce>? similarProducts = null;
    public bool IsLoading { get; set; }
    public string? ErrorMessage { get; set; }
    public bool clickedShareButton { get; set; } = false;

    public bool IsSameUser { get; set; } = true;

    public bool IsUserSuspended { get; set; } = false;
    public bool IsBlockedByUser { get; set; } = false;
    public bool ShowSignalerModal { get; set; } = false;
    public string SignalementRaison { get; set; } = string.Empty;
    public bool IsSubmittingReport { get; set; } = false;

    public DetailAnnonceViewModel(IAnnonceService annonceService,
        IFavorisService<Favoris> favorisService, IAuthService authService,
        IUtilisateurService utilisateurService,
        IConversationService<Conversation> conversationService,
        ClipboardService clipboardService, NavigationManager navigationManager, IMediasService<Photo> mediasService
        , IVisualisationService visualisationService, ISignalementService signalementService)
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
    }

    public async Task LoadAnnonceDetailAsync(int id)
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            AnnonceDetail = await _annonceService.GetAnnonceDetailById(id);
            if (AnnonceDetail == null)
            {
                ErrorMessage = "Annonce introuvable";
            }
            else
            {
                await StartVisualisationTimer(AnnonceDetail.AnnonceId);
            }

            utilisateurAnnonce = await _utilisateurService.GetUserById(AnnonceDetail.UtilisateurId);
            IsBlockedByUser = utilisateurAnnonce.blockedByCurrentUser;
            if (utilisateurAnnonce == null || utilisateurAnnonce.Statut == "Suspendu")
                IsUserSuspended = true;

            Utilisateur? utilisateur = await _authService.GetCurrentUserAsync();
            if (utilisateur != null && utilisateurAnnonce != null &&
                utilisateur.UtilisateurId == utilisateurAnnonce.UtilisateurId)
                IsSameUser = true;

            else
                IsSameUser = false;

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
    }
    public async Task GetSimilarProductsAsync()
    {
        //// For future try to make a good filter that grabs the infos, also using GetByIds etc
        //// For future try to make a good filter that grabs the infos, also using GetByIds etc
        //FilterDTO filterDTO = new FilterDTO();
        //filterDTO.MotCle = AnnonceDetail.Title;

        //similarProducts = await _annonceService.GetAnnonceByFilter(filterDTO, page: 1, pageSize: 3);
    }
    public async Task<bool> CheckLoginStatus()
    {
        if (await _authService.GetCurrentUserAsync() != null)
            return true;
        return false;
    }
    public async Task ToggleFavorite(int id)
    {
        if (CheckLoginStatus == null)
        {
            _navigationManager.NavigateTo("/login");
            return;
        }
        AnnonceDetail annonce = await _annonceService.GetAnnonceDetailById(id);

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
        if (CheckLoginStatus == null)
        {
            _navigationManager.NavigateTo("/login");
            return;
        }

        // check if conversation already exists, if not then create and send to page
        var conv = await _conversationService.GetOrCreateConversation(AnnonceDetail.AnnonceId);
        _navigationManager.NavigateTo($"/messages?conversationId={conv.ConversationId}");
    }

    public void MakeOffer()
    {
        // TODO: Open make offer dialog
        // open something like review form thing for the avis
        // but he inserts the price, then checks if conversation exists, if not creates and 
        // creates a new message too of type Proposition.
    }

    public void BuyProduct()
    {
        // TODO go to page payment and ye
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
        ShowSignalerModal = !ShowSignalerModal;
    }
    public async Task SubmitReport()
    {
        IsSubmittingReport = true;
        try
        {
            SignalementAnnonceCreate newReport = new SignalementAnnonceCreate
            {
                SignalementMotif = SignalementRaison,
                AnnonceSignaleeId = AnnonceDetail.AnnonceId,
            };

            SignalementCreate reuslt = await _signalementService.CreateSignalement(newReport);
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
}
