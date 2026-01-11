using System.Collections.ObjectModel;
using FrontBlazor.Services;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Shared.DTO.Notification;
using Shared.DTO.Utilisateur;

namespace FrontBlazor.ViewModel.Generic;

public class ClientBaseViewModel : IAsyncDisposable
{
    protected readonly IAuthService  _authService;
    protected readonly NavigationManager _nav;
    private SearchAnnonceViewModel? _searchViewModel;
    private readonly INotificationService _notificationService;
    private readonly ISignalRService _signalRService;

    public int NotificationCount { get; set; }
    public int MessageCount { get; set; }
    
    #region variables
    public bool IsLoggedIn { get; set; }
    public event Action? OnStateChanged;
    public CurrentUtilisateurDTO utilisateur { get; set; }
    public ObservableCollection<NotificationDTO> notifications { get; set; }
    public bool showDropdown;
    public string SearchQuery { get; set; } = "";
    public event Action<string>? OnSearchQueryChanged;
    public bool showDropDownNotification { get; set; }
    public bool LoadingNotifications { get; set; }
    public bool IsLoadingBase { get; set; }
    public bool IsLoading { get; set; }
    public string RoleUtilisateur { get; set; }

    #endregion
    public ClientBaseViewModel(
        NavigationManager nav,
        IAuthService authService,
        ISignalRService signalRService,
        INotificationService notificationService
        )
    {
        _notificationService =  notificationService;
        _nav = nav;
        _signalRService = signalRService;
        _authService = authService;
        _signalRService.OnNotificationCountUpdated += HandleNotificationCountUpdate;
        _signalRService.OnMessageCountUpdated += HandleUnReadMesssageCountUpdate;

    }
    
    protected void NotifyStateChanged()
    {
        OnStateChanged?.Invoke();
    }
    public virtual async Task LoadAsync()
    {
        IsLoading = true;
        showDropdown = false;
        showDropDownNotification =  false;
        IsLoadingBase = true;
        var user =  await _authService.GetCurrentUserAsync();
        if (user != null)
        {
            NotificationCount = user.NotificationsCount;
            MessageCount = user.MessagesCount;
            IsLoggedIn = true;
            utilisateur = user;
            RoleUtilisateur = user.RoleUtilisateur;
            await _signalRService.StartNotificationHubAsync();
            if (user.Statut != "Actif")
            {
                _nav.NavigateTo("/Sanction");
            }
        }
        else
        {
            IsLoggedIn = false;
        }
        IsLoadingBase = false;
        NotifyStateChanged();
    }
    public void ToggleDropdown()
    {
        showDropdown = !showDropdown;
        showDropDownNotification = false;
            
    }

    public async Task SetSearchViewModel(SearchAnnonceViewModel searchViewModel)
    {
        _searchViewModel = searchViewModel;
    }
    public async Task ToggleDropDownNotification()
    {
        showDropDownNotification = !showDropDownNotification;
        showDropdown = false;
        LoadingNotifications = true;
        NotificationCount = 0;
        NotifyStateChanged();

        if (showDropDownNotification)
        {
            var result = await _notificationService.GetAllAsync();
            foreach (var notif in result)
            {
                switch (notif)
                {
                    case NotificationAdminDTO ad:
                        Console.WriteLine(ad.AdminText);
                        break;
                }
            }
            notifications = result ?? new ObservableCollection<NotificationDTO>();
            NotifyStateChanged(); 
        }
        LoadingNotifications = false;
    }
    public void NavigateToSearch()
    {
        if (!string.IsNullOrWhiteSpace(SearchQuery))
            _nav.NavigateTo($"/search?q={Uri.EscapeDataString(SearchQuery)}");
        else
            _nav.NavigateTo("/search");
    }

    public void NavigateToProfile(string login)
    {
        ToggleDropdown();
        _nav.NavigateTo($"/profile/{login}", true);
    }

    public void NavigateToPage(string page)
    {
        ToggleDropdown();
        _nav.NavigateTo(page, true);
    }

    public async Task OnSearchInputChanged()
    {
        if (_nav.Uri.Contains("/search") && _searchViewModel != null)
        {
            await _searchViewModel.OnSearchInput(SearchQuery);
        }
    }
    public async Task HandleSearchInput(ChangeEventArgs e)
        {
            SearchQuery = e.Value?.ToString() ?? string.Empty;
            await OnSearchInputChanged();
        }

        public void HandleSearchKeyDown(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
                NavigateToSearch();
        }

        public async Task HandleLogout()
        {
            IsLoggedIn = false;
            showDropdown = false;
        
            await _signalRService.StopAsync();
            await _authService.LogoutAsync();
            _nav.NavigateTo(_nav.Uri, true);
        }
        
        private void HandleNotificationCountUpdate(int count)
        {
            NotificationCount = count;
            NotifyStateChanged();
        }

        private void HandleUnReadMesssageCountUpdate(int count)
        {
            MessageCount = count;
            NotifyStateChanged();
        }
        public async ValueTask DisposeAsync()
        {
            _signalRService.OnNotificationCountUpdated -= HandleNotificationCountUpdate;
            await _signalRService.StopAsync();
        }

        public async Task DeleteNotification(int id)
        {
            await _notificationService.DeleteNotification(id);
            var notificationToRemove = notifications.FirstOrDefault(n => n?.NotificationId == id);
            if (notificationToRemove != null)
            {
                notifications.Remove(notificationToRemove);
                NotifyStateChanged();
            }
        }

        public async Task HandleNotificationClick(NotificationDTO notification)
        {
            if (!notification.EstLu)
            {
                notification.EstLu = true;
            }
            showDropDownNotification = false;
            NotifyStateChanged();
            switch (notification)
            {
                case NotificationMessageDTO notifMessage:
                    if (String.IsNullOrEmpty(notifMessage.ConversationId.ToString()))
                    {
                        _nav.NavigateTo($"/messages?conversationId={notifMessage.ConversationId}");
                    }
                    else
                    {
                        _nav.NavigateTo("/messages");
                    }
                    break;

                case NotificationModificationAnnonceDTO notifModif:
                    _nav.NavigateTo($"/product/{notifModif.ModificationAnnonceId}");
                    break;

                case NotificationNouvelleAnnonceDTO notifNouvelle:
                    _nav.NavigateTo($"/product/{notifNouvelle.NouvelleAnnonceId}");
                    break;

                case NotificationAvertissementDTO:
                    break;

                case NotificationAdminDTO:
                    break;
                case NotificationPropositionDTO notifProposition:
                    _nav.NavigateTo($"/messages?conversationId={notifProposition.ConversationId}");
                    break;
            }
            await DeleteNotification(notification.NotificationId);
        }
        public void CloseAllDropdowns()
        {
            showDropdown = false;
            showDropDownNotification = false;
            NotifyStateChanged();
        }
}