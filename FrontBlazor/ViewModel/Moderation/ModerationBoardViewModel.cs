using Shared.DTO;
using FrontBlazor.Pages.Moderation;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace FrontBlazor.ViewModel;

public class ModerationBoardViewModel : ModerationViewModel
{
    private readonly IModerationDashboardService _moderationDashboardService;
    private readonly NavigationManager _nav;
    public ModerationBoardViewModel(
        IModerationDashboardService moderationDashboardService,
        IAuthService authService,
        NavigationManager nav)
        : base( authService, nav)
    {
        _moderationDashboardService = moderationDashboardService;
        _nav = nav;
    }
    
    public DashBoardStatistics DashBoardStatistics { get; set; }
    public List<ActivityDTO> Activity { get; set; }
    public async override Task LoadAsync()
    {
        await base.LoadAsync();
        if (IsModerator)
        {
            Activity = await _moderationDashboardService.GetActivity();
            DashBoardStatistics = await _moderationDashboardService.GetStatistics();
            IsLoading = false;
        }
    }

    public async Task NavigateToSignalement(int id)
    {
        _nav.NavigateTo($"/moderation/signalements/{id}");
    }

    public async Task NavigateToRestauration(int id)
    {
        _nav.NavigateTo($"moderation/demande-restauration/{id}");
    }
}