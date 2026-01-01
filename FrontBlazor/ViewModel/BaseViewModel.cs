using FrontBlazor.Services.GenericIServices;
using Microsoft.AspNetCore.Components;

namespace FrontBlazor.ViewModel;

public abstract class BaseViewModel
{
    protected readonly IAuthService  _authService;
    protected readonly NavigationManager _nav;

    public event Action? OnStateChanged;


    public BaseViewModel(
        IAuthService authService,
        NavigationManager nav
        )
    {
        _authService = authService;
        _nav = nav;
    }

    protected void NotifyStateChanged()
    {
        OnStateChanged?.Invoke();
    }

    public async Task VerifiyAccountAsync()
    {
        var user =  await _authService.GetCurrentUserAsync();
        if (user != null)
        {
            if (user.StatutId != 1)
            {
                _nav.NavigateTo("/Sanction");
            }  
        }
    }
}