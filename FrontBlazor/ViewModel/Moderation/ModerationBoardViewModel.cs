using Shared.DTO;
using FrontBlazor.Pages.Moderation;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FrontBlazor.ViewModel;

public class ModerationBoardViewModel : ModerationViewModel
{
    private readonly IModerationDashboardService _moderationDashboardService;
    public ModerationBoardViewModel(
        IModerationDashboardService moderationDashboardService,
        IAuthService authService,
        NavigationManager nav)
        : base( authService, nav)
    {
        _moderationDashboardService = moderationDashboardService;
    }
    
    public DashBoardStatistics DashBoardStatistics { get; set; }
    public async override Task LoadAsync()
    {
        base.LoadAsync();
        IsLoading = true;
        DashBoardStatistics = await _moderationDashboardService.GetStatistics();
        IsLoading = false;
    }
}