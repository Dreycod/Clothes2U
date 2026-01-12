using FrontBlazor.Pages.Moderation;
using FrontBlazor.Services.Interfaces;
using FrontBlazor.Services.WebService;
using Microsoft.AspNetCore.Components;
using Shared.DTO.SupportTicket;

namespace FrontBlazor.ViewModel.Moderation.Support;

public class TicketDetailViewModel : ModerationViewModel
{
    private readonly SupportWebService _service;
    private readonly NavigationManager _nav;
    
    public SupportTicketDetailViewDTO TicketDetail { get; set; }
    public int Id { get; set; }
    
    public TicketDetailViewModel(
        SupportWebService service,
        IAuthService authService,
        NavigationManager nav)
        : base(authService, nav)
    {
        _nav = nav;
        _service = service;
    }
    
    public override async Task LoadAsync()
    {
        IsLoading = true;
        await base.LoadAsync();
        try
        {
            TicketDetail = await _service.GetTicketById(Id);
        }
        finally
        {
            IsLoading = false;
        }
    }
    
}