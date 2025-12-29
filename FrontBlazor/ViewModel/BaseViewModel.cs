using FrontBlazor.Services.GenericIServices;
using Microsoft.AspNetCore.Components;

namespace FrontBlazor.ViewModel;

public abstract class BaseViewModel
{
    private readonly IAuthService  _authService;
    private readonly NavigationManager _nav;
    
    public BaseViewModel(
        IAuthService authService,
        NavigationManager nav
        )
    {
        _authService = authService;
        _nav = nav;
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