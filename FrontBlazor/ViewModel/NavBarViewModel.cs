using System.Collections.ObjectModel;
using Shared.DTO;
using Shared.DTO.Notification;
using Shared.DTO.Utilisateur;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace FrontBlazor.ViewModel
{
    public class NavBarViewModel
    {
        private readonly IAuthService _authService;
        private readonly INotificationService _notificationService;
        private readonly NavigationManager _nav;
        private SearchAnnonceViewModel? _searchViewModel;

        public UtilisateurDTO utilisateur { get; set; }
        public ObservableCollection<NotificationDTO> notifications { get; set; } =  new ObservableCollection<NotificationDTO>();
        public bool IsLoading { get; set; }
        public bool IsConnected { get; set; }
        public bool showDropdown;
        public string SearchQuery { get; set; } = "";
        public event Action<string>? OnSearchQueryChanged;

        public NavBarViewModel(
            IAuthService authService,
            NavigationManager nav,
            INotificationService notificationService
            )
        {
            _authService = authService;
            _nav = nav;
            _notificationService = notificationService;
        }

        public void SetSearchViewModel(SearchAnnonceViewModel searchViewModel)
        {
            _searchViewModel = searchViewModel;
        }

        public void ToggleDropdown()
        {
            showDropdown = !showDropdown;
            showDropDownNotification = false;
            
        }
        public bool showDropDownNotification { get; set; }
        public bool LoadingNotifications { get; set; }

        public async Task ToggleDropDownNotification()
        {
            showDropDownNotification = !showDropDownNotification;
            showDropdown = false;
            LoadingNotifications = true;

            NotifyStateChanged();

            if (showDropDownNotification)
            {
                var result = await _notificationService.GetAllAsync();
                notifications = result ?? new ObservableCollection<NotificationDTO>();
                NotifyStateChanged(); 
            }
            LoadingNotifications = false;
        }


        public virtual async Task LoadAsync()
        {
            showDropdown = false;
            showDropDownNotification =  false;
            IsLoading = true;
            utilisateur = await _authService.GetCurrentUserAsync();
            if (utilisateur == null)
            {
                IsConnected = false;
            }
            else
            {
                IsConnected = true;
            }
            IsLoading = false;
        }

        public void NavigateToSearch()
        {
            if (!string.IsNullOrWhiteSpace(SearchQuery))
                _nav.NavigateTo($"/search?q={Uri.EscapeDataString(SearchQuery)}");
            else
                _nav.NavigateTo("/search");
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
            IsConnected = false;
            showDropdown = false;
            await _authService.LogoutAsync();
            _nav.NavigateTo(_nav.Uri, true);
        }
        
        public event Action? OnStateChanged;

        private void NotifyStateChanged()
        {
            OnStateChanged?.Invoke();
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
            }
            await DeleteNotification(notification.NotificationId);
        }
    }
}