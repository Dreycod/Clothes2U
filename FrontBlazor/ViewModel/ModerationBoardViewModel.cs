using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;
using Microsoft.AspNetCore.Components;

namespace FrontBlazor.ViewModel;

public class ModerationBoardViewModel
{
    private readonly IAuthService _authService;
    private readonly NavigationManager _nav;
    private readonly ISignalementService  _signalementService;

    public ModerationBoardViewModel(ISignalementService signalementService, IAuthService authService,  NavigationManager nav)
    {
        _signalementService = signalementService;
        _authService = authService;
        _nav = nav;
    }

    public async Task LoadAsync()
    {
        Utilisateur user = await _authService.GetCurrentUserAsync();
        Console.WriteLine(user.RoleUtilisateur);
        if (user == null || user.RoleUtilisateur != "")
        {
            _nav.NavigateTo("/");
        }
        Console.WriteLine("ET OUIIII");
    }
}