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

        public List<SupportTicketViewDTO> Tickets { get; private set; } = new();
        public bool IsReplying { get; private set; }
        public string ReplyMessage { get; set; } = "";
        public int CurrentTicketId { get; private set; }

        public SupportTicketsViewModel(
            SupportWebService service,
            IAuthService authService,
            NavigationManager nav)
            : base(authService, nav)
        {
            _service = service;
        }

        public override async Task LoadAsync()
        {
            await base.LoadAsync();
            Tickets = await _service.GetOpenTicketsAsync();
        }

        public void OpenReply(int ticketId)
        {
            CurrentTicketId = ticketId;
            ReplyMessage = "";
            IsReplying = true;
        }

        public async Task SendReplyAsync()
        {
            await _service.ReplyAsync(new()
            {
                TicketId = CurrentTicketId,
                Message = ReplyMessage
            });

            IsReplying = false;
            Tickets = await _service.GetOpenTicketsAsync();
        }
    }
}
