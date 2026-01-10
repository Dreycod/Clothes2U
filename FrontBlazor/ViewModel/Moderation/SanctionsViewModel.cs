using FrontBlazor.Pages.Moderation;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components;
using Shared.DTO.Decision;

namespace FrontBlazor.ViewModel;

public class SanctionsViewModel : ModerationViewModel
{
    private readonly IDecisionService  _decisionService;

    public SanctionsViewModel(IDecisionService decisionService, 
        IAuthService authService,
        NavigationManager nav): base(authService, nav)
    {
        _decisionService = decisionService;
    }
    public bool IsLoading { get; set; }
    public List<DecisionDTO> Decisions { get; set; } = new List<DecisionDTO>();
    
    public override async Task LoadAsync()
    {
        IsLoading = true; 
        base.LoadAsync();
        Decisions = await _decisionService.GetAllDecisionByModeratorIdAsync();
        IsLoading = false;
    }
}