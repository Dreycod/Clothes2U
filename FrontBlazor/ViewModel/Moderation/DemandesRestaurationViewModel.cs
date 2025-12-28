using FrontBlazor.Pages.Moderation;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Shared.DTO.DemandeRestauration;

namespace FrontBlazor.ViewModel.Moderation;

public class DemandesRestaurationViewModel : ModerationViewModel
{
    private readonly IDemandeRestaurationService _demandeRestaurationService;
    private readonly NavigationManager _nav;

    public DemandesRestaurationViewModel(
        IAuthService  authService,
        NavigationManager nav,
        IDemandeRestaurationService demandeRestaurationService
        ) :  base(authService, nav)
    {
        _demandeRestaurationService = demandeRestaurationService;
        _nav = nav;
    }
    
    public bool IsLoading { get; set; }
    public List<DemandeRestaurationDTO> DemandeRestaurations { get; set; } = new();

    public override async Task LoadAsync()
    {
        IsLoading = true;
        base.LoadAsync();
        DemandeRestaurations = await _demandeRestaurationService.GetAllDemandeRestaurations();
        IsLoading = false;
    }
    public event Action? OnStateChanged;

    private void NotifyStateChanged()
    {
        OnStateChanged?.Invoke();
    }

    public void NavigateTo(string url)
    {
        _nav.NavigateTo(url);
    }
}