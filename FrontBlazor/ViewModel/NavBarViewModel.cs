using FrontBlazor.Models;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace FrontBlazor.ViewModel
{
    public class NavBarViewModel
    {
        private readonly IAuthService _authService;
        private readonly NavigationManager _nav;
        private SearchAnnonceViewModel? _searchViewModel;

        public Utilisateur utilisateur { get; set; }
        public bool IsLoading { get; set; }
        public bool IsConnected { get; set; }
        public bool showDropdown;
        public string SearchQuery { get; set; } = "";
        public event Action<string>? OnSearchQueryChanged;

        public NavBarViewModel(
            IAuthService authService,
            NavigationManager nav
            )
        {
            _authService = authService;
            _nav = nav;
        }

        public void SetSearchViewModel(SearchAnnonceViewModel searchViewModel)
        {
            _searchViewModel = searchViewModel;
        }

        public void ToggleDropdown()
        {
            showDropdown = !showDropdown;
        }

        public virtual async Task LoadAsync()
        {
            showDropdown = false;
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
    }
}
