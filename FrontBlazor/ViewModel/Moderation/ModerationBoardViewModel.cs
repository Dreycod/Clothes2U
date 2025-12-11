using FrontBlazor.Models;
using FrontBlazor.Pages.Moderation;
using FrontBlazor.Services.GenericIServices;
using Microsoft.AspNetCore.Components;

namespace FrontBlazor.ViewModel;

public class ModerationBoardViewModel : ModerationViewModel
{
    private readonly ISignalementService  _signalementService;
    public ModerationBoardViewModel(
        ISignalementService signalementService,
        IAuthService authService,
        NavigationManager nav)
        : base( authService, nav)
    {
    }
}