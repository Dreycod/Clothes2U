using FrontBlazor.Pages.Moderation;
using FrontBlazor.Services.Interfaces;
using FrontBlazor.Services.WebService;
using Microsoft.AspNetCore.Components;
using Shared.DTO.SupportTicket;

namespace FrontBlazor.ViewModel.Moderation.Support
{
    public class SupportTicketsViewModel : ModerationViewModel
    {
        private readonly SupportWebService _service;
        private readonly NavigationManager _nav;

        public List<SupportTicketViewDTO> Tickets { get; private set; } = new();

        public SupportTicketsViewModel(
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
            Tickets = await _service.GetOpenTicketsAsync();
            IsLoading = false;
            foreach (var supportTicketViewDto in Tickets)
            {
                Console.WriteLine(supportTicketViewDto.Title);
            }
        }

        public async Task LoadPendingTickets()
        {
            Tickets = await _service.GetPendingTicketsAsync();
        }

        public async Task OpenDetails(int ticketId)
        {
            _nav.NavigateTo($"/moderation/TicketDetails/{ticketId}");
        }

        
    }
}
