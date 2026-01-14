using FrontBlazor.Pages.Moderation;
using FrontBlazor.Services.Interfaces;
using FrontBlazor.Services.WebService;
using Microsoft.AspNetCore.Components;
using Shared.DTO.SupportTicket;
using Shared.Enums;

namespace FrontBlazor.ViewModel.Moderation.Support
{
    public class SupportTicketsViewModel : ModerationViewModel
    {
        private readonly SupportWebService _service;
        private readonly NavigationManager _nav;
        private readonly IAuthService _authService;

        public List<SupportTicketViewDTO> Tickets { get; private set; } = new();

        public SupportTicketsViewModel(
            SupportWebService service,
            IAuthService authService,
            NavigationManager nav)
            : base(authService, nav)
        {
            _nav = nav;
            _service = service;
            _authService = authService;
        }

        public StatusTicketEnum StatusTicket { get; set; }
        public override async Task LoadAsync()
        {
            IsLoading = true;
            await base.LoadAsync();
            await LoadOpenTickets();
            IsLoading = false;
            
        }

        public async Task LoadOpenTickets()
        {
            try
            {
                IsLoading = true;
                StatusTicket = StatusTicketEnum.OPEN;
                Tickets = await _service.GetOpenTicketsAsync();
                IsLoading = false;
            }
            catch (Exception ex)
            {
                _authService.LogoutAsync();
                _nav.NavigateTo("/login", forceLoad: true);
            }
        }
        public async Task LoadPendingTickets()
        {
            try
            {
                IsLoading = true;
                StatusTicket = StatusTicketEnum.PENDING;
                Tickets = await _service.GetPendingTicketsAsync();
                IsLoading = false;
            }
            catch (Exception ex)
            {
                _authService.LogoutAsync();
                _nav.NavigateTo("/login", forceLoad: true);
            }
            
        }
        public async Task LoadClosedTickets()
        {
            try
            {
                IsLoading = true;
                StatusTicket = StatusTicketEnum.CLOSED;
                Tickets = await _service.GetClosedTicketsAsync();
                IsLoading = false;
            }
            catch (Exception ex)
            {
                _authService.LogoutAsync();
                _nav.NavigateTo("/login", forceLoad: true);
            }
            
        }

        public async Task OpenDetails(int ticketId)
        {
            _nav.NavigateTo($"/moderation/TicketDetails/{ticketId}");
        }

        
    }
}
