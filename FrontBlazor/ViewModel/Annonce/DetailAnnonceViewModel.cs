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
using System.Runtime.CompilerServices;

namespace FrontBlazor.ViewModel;

public class DetailAnnonceViewModel : ClientBaseViewModel
{
    private readonly IAnnonceService _annonceService;
    private readonly IFavorisService<FavorisDTO> _favorisService;
    private readonly IConversationService<ConversationDTO> _conversationService;
    private readonly IUtilisateurService _utilisateurService;
    private readonly NavigationManager _navigationManager;
    private readonly ClipboardService _clipboardService;
    private readonly IMediasService _mediaService;
    private readonly IVisualisationService _visualisationService;
    private readonly ISignalementService _signalementService;

    private CancellationTokenSource? _viewTimerCts;

    public AnnonceDetailDTO? AnnonceDetail { get; set; }
    public UtilisateurViewDTO? utilisateurAnnonce { get; set; }
    public List<AnnonceDTO>? similarAnnonces = null;
    public bool IsLoading { get; set; }
    public string? ErrorMessage { get; set; }
    public bool IsSameUser { get; set; } = false;

    public bool IsUserSuspended { get; set; } = false;
    public bool IsBlockedByUser { get; set; } = false;
    public bool ShowSignalerModal { get; set; } = false;
    public string SignalementRaison { get; set; } = string.Empty;
    public bool IsSubmittingReport { get; set; } = false;
    public bool IsLoadingSimilar { get; set; }

    public bool ShowDeleteProductModal { get; set; } = false;
    public bool IsSubmittingDelete { get; set; } = false;

    public bool ShowImagePreview { get; set; } = false;
    public string ImagePreview { get; set; } = string.Empty;
    public bool ShowActionsDropdown { get; set; } = false;
    public bool ShowPauseResumeModal { get; set; } = false;
    public bool IsSubmittingPauseResume { get; set; } = false;

    public bool Show3DViewer { get; set; } = false;
    public bool Has3DModel { get; set; } = false;
    public DetailAnnonceViewModel(
        IAnnonceService annonceService,
        IFavorisService<FavorisDTO> favorisService, 
        IAuthService authService,
        IUtilisateurService utilisateurService,
        IConversationService<ConversationDTO> conversationService,
        ClipboardService clipboardService,
        NavigationManager navigationManager, 
        IMediasService mediasService,
        IVisualisationService visualisationService, 
        ISignalementService signalementService,
        INotificationService notificationService,
        ISignalRService notificationHubService
        )
        : base(navigationManager, authService, notificationHubService, notificationService)
    
    {
        _annonceService = annonceService;
        _favorisService = favorisService;
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
        AnnonceDetail = null;
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

            if (AnnonceDetail.SousCategorie == "T-shirt")
            {
                Has3DModel = true;
            }

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

    public async void ToggleDeleteProductModal()
    {
        if (utilisateur == null)
        {
            _navigationManager.NavigateTo("/login");
            return;
        }

        ShowDeleteProductModal = !ShowDeleteProductModal;
    }

    public async Task DeleteProduct()
    {
        IsSubmittingDelete = true;
        try
        {
            await _annonceService.DeleteAnnonce(AnnonceDetail.AnnonceId);
            _navigationManager.Refresh();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la suppression de l'annonce : {ex.Message}");
        }
        finally
        {
            IsSubmittingDelete = false;
            ShowDeleteProductModal = false;
        }
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

    public void ToggleActionsDropdown()
    {
        ShowActionsDropdown = !ShowActionsDropdown;
        NotifyStateChanged();
    }
    public void TogglePauseResumeModal()
    {
        if (utilisateur == null)
        {
            _navigationManager.NavigateTo("/login");
            return;
        }
        ShowPauseResumeModal = !ShowPauseResumeModal;
        ShowActionsDropdown = false;
        NotifyStateChanged();
    }
    public void ModifierAnnonce()
    {
        ShowActionsDropdown = false;
        NotifyStateChanged();
        _navigationManager.NavigateTo($"/update-article/{AnnonceDetail.AnnonceId}");
    }

    public async Task PauserReprendreAnnonce()
    {
        ShowActionsDropdown = false;
        NotifyStateChanged();
        string action = "";

        try
        {
            switch (AnnonceDetail.StatutAnnonceId)
            {
                case 5:
                    await _annonceService.ChangeEtatAnnonce(AnnonceDetail.AnnonceId, 1);
                    break;
                case 1:
                    await _annonceService.ChangeEtatAnnonce(AnnonceDetail.AnnonceId, 5);
                    break;
                default:
                    throw new InvalidOperationException("Action inconnue pour l'annonce.");
            }
            AnnonceDetail.StatutAnnonceId = action == "pauser" ? 5 : 1;
            NotifyStateChanged();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la pause de l'annonce : {ex.Message}");
        }
        finally
        {
            IsSubmittingPauseResume = false;
            ShowPauseResumeModal = false;
            _navigationManager.Refresh();
            NotifyStateChanged();
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
            List<AnnonceDTO> newAnnonces = await _annonceService.GetSimilarAnnonces(AnnonceDetail.AnnonceId, PageNumber + 1, 4);
            if (newAnnonces.Count > 0)
            {
                PageNumber++;
                similarAnnonces = newAnnonces;
                NotifyStateChanged();
            }
        }
    }
    public async Task ToggleImagePreview(int? photoId = 0)
    {
        if (photoId == 0)
        {
            ShowImagePreview = false;
        }
        else
        {
            ImagePreview = _mediaService.GetPhotoUrl((int)photoId);
            ShowImagePreview = true;
        }
    }
    public void Toggle3DViewer()
    {
        Show3DViewer = !Show3DViewer;
    }
}
