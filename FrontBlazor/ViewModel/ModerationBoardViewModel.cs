using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;
using Microsoft.AspNetCore.Components;

namespace FrontBlazor.ViewModel;

public class ModerationBoardViewModel
{
    private readonly IAuthService _authService;
    private readonly NavigationManager _nav;
    private readonly ISignalementService  _signalementService;
    public bool IsLoading { get; set; }

    public ModerationBoardViewModel(ISignalementService signalementService, IAuthService authService,  NavigationManager nav)
    {
        _signalementService = signalementService;
        _authService = authService;
        _nav = nav;
    }

    public async Task LoadAsync()
    {
        IsLoading = true;
        Utilisateur user = await _authService.GetCurrentUserAsync();
        if (user == null || user.RoleUtilisateur != "Admin" && user.RoleUtilisateur != "Modérateur")
        {
            _nav.NavigateTo("/");
        }
        IsLoading = false;
    }
}