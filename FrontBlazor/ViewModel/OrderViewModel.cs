using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Components;
using FrontBlazor.Services.Interfaces;
using FrontBlazor.ViewModel.Generic;
using Shared.DTO;
using Shared.DTO.Utilisateur;

namespace FrontBlazor.ViewModel;

public class OrderViewModel : ClientBaseViewModel, IDisposable
{
    private readonly IOrderService _orderService;
    private readonly IAuthService _authService;
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

    public OrderViewModel(
        IOrderService orderService,
        IAuthService authService,
        NavigationManager nav,
        INotificationService notificationService,
        NavigationManager navigationManager,
        ISignalRService signalRService) : base(navigationManager, authService,signalRService, notificationService)
    {
        _orderService = orderService;
        _authService = authService;
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
}

public enum OrderTab
{
    Purchases,
    Sales
}