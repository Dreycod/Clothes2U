using FrontBlazor.Services.Interfaces;
using FrontBlazor.ViewModel.Generic;
using Microsoft.AspNetCore.Components;
using Shared.DTO;
using Shared.DTO.Conversation;
using Shared.DTO.NoteUtilisateur;
using Shared.DTO.Utilisateur;
using System.Collections.ObjectModel;

namespace FrontBlazor.ViewModel;

public class OrderViewModel : ClientBaseViewModel, IDisposable
{
    private readonly IOrderService _orderService;
    private readonly IAuthService _authService;
    private readonly INoteUtilisateurService _noteUtilisateurService;
    private readonly IConversationService<ConversationDTO> _conversationService;
    private readonly NavigationManager _nav;

    public List<OrderDTO> PurchasedOrders { get; private set; } = new();
    public List<OrderDTO> SoldOrders { get; private set; } = new();
    //public UtilisateurDTO? CurrentUser { get; private set; }
    
    // États
    public bool IsLoading { get; private set; }
    public string? ErrorMessage { get; private set; }
    public OrderTab CurrentTab { get; private set; } = OrderTab.Purchases;
    
    // Statistiques
    public OrderStatsDTO? PurchaseStats { get; private set; }
    public OrderStatsDTO? SalesStats { get; private set; }
    
    // Filtres
    public string SearchQuery { get; set; } = "";
    public string StatusFilter { get; set; } = "Tous";

    public event Action? OnChange;
    #region Avis
    public bool ShowAddReviewModal { get; set; } = false;
    public int SelectedRating { get; set; } = 0;
    public string ReviewComment { get; set; } = string.Empty;
    public string ReviewErrorMessage { get; set; } = string.Empty;
    public bool IsSubmittingReview { get; set; } = false;
    #endregion

    public OrderViewModel(
        IOrderService orderService,
        IAuthService authService,
        NavigationManager nav,
        INotificationService notificationService,
        NavigationManager navigationManager,
        INoteUtilisateurService noteUtilisateurService,
        IConversationService<ConversationDTO> conversationService,
        ISignalRService signalRService) : base(navigationManager, authService,signalRService, notificationService)
    {
        _orderService = orderService;
        _authService = authService;
        _noteUtilisateurService = noteUtilisateurService;
        _nav = nav;
    }
    
    public override async Task LoadAsync()
    {
        IsLoading = true;
        ErrorMessage = null;
        NotifyStateChanged();
        await base.LoadAsync();
        if (utilisateur == null)
        {
            _nav.NavigateTo("/");
        }
        
        try
        {

            PurchasedOrders = await _orderService.GetUserOrdersAsync();
            SoldOrders = await _orderService.GetSellerOrdersAsync();
            PurchaseStats = await _orderService.GetUserOrderStatsAsync(utilisateur.UtilisateurId);
            
            // Calculer les stats de vente
            SalesStats = new OrderStatsDTO
            {
                TotalCommandes = SoldOrders.Count,
                CommandesEnCours = SoldOrders.Count(o => o.StatutCommandeId == 1 || o.StatutCommandeId == 2),
                CommandesLivrees = SoldOrders.Count(o => o.StatutCommandeId == 3),
                MontantTotal = SoldOrders.Sum(o => o.MontantTotal)
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[OrdersVM] ❌ Error loading: {ex.Message}");
            ErrorMessage = "Une erreur est survenue lors du chargement";
        }
        finally
        {
            IsLoading = false;
            NotifyStateChanged();
        }
    }

    public void SwitchTab(OrderTab tab)
    {
        CurrentTab = tab;
        NotifyStateChanged();
    }

    public List<OrderDTO> GetFilteredOrders()
    {
        var orders = CurrentTab == OrderTab.Purchases 
            ? PurchasedOrders.ToList() 
            : SoldOrders.ToList();

        // Filtrer par statut
        if (StatusFilter != "Tous")
        {
            orders = orders.Where(o => o.StatutCommandeLibelle == StatusFilter).ToList();
        }

        // Filtrer par recherche
        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var query = SearchQuery.ToLower();
            orders = orders.Where(o => 
                o.TitreAnnonce.ToLower().Contains(query) ||
                o.CommandeId.ToString().Contains(query)
            ).ToList();
        }

        return orders.OrderByDescending(o => o.DateCommande).ToList();
    }

    public void NavigateToOrderDetails(int orderId)
    {
        _nav.NavigateTo($"/order/{orderId}");
    }

    public string GetStatusClass(int status)
    {
        return status switch
        {
            1 => "status-paid",
            2 => "status-shipped",
            3 => "status-delivered",
            4 => "status-cancelled",
            _ => "status-pending"
        };
    }

    public string GetStatusIcon(int status)
    {
        return status switch
        {
            1 => "bi-credit-card-fill",
            2 => "bi-truck",
            3 => "bi-check-circle-fill",
            4 => "bi-x-circle-fill",
            _ => "bi-clock-fill"
        };
    }

    private void NotifyStateChanged() => OnChange?.Invoke();

    public void Dispose()
    {
        // Cleanup si nécessaire
    }
    
    public void ShowAddReview()
    {
        ShowAddReviewModal = true;
        ReviewErrorMessage = string.Empty;
        SelectedRating = 0;
        ReviewComment = string.Empty;
    }

    public void CloseAddReview()
    {
        ShowAddReviewModal = false;
    }

    public void SetRating(int rating)
    {
        SelectedRating = rating;
    }

    public async Task SubmitReview(OrderDTO order)
    {

        if (utilisateur == null) return;

        if (SelectedRating <= 0)
        {
            ReviewErrorMessage = "Veuillez sélectionner une note.";
            return;
        }

        if (ReviewComment.Length < 10)
        {
            ReviewErrorMessage = "Le commentaire doit contenir au moins 10 caractères.";
            return;
        }

        IsSubmittingReview = true;
        ReviewErrorMessage = string.Empty;

        try
        {
            NoteUtilisateurCreateDTO newReview = new NoteUtilisateurCreateDTO
            {
                Note = SelectedRating,
                Commentaire = ReviewComment,
                CibleId = order.CommandeId
            };

            var result = await _noteUtilisateurService.AddNoteUtilisateur(newReview);
            if (result.Success)
            {
                CloseAddReview();
            }
            else
            {
                ReviewErrorMessage = result.ErrorMessage ?? "Erreur lors de l'envoi de l'avis.";
                NotifyStateChanged();
            }
        }
        catch
        {
            ReviewErrorMessage = "Erreur lors de l'envoi de l'avis.";
            NotifyStateChanged();
        }
        finally
        {
            IsSubmittingReview = false;
            NotifyStateChanged();
        }
    }
}

public enum OrderTab
{
    Purchases,
    Sales
}