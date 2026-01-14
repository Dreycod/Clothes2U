using API.Models.EntityFramework;
using API.Models.Repository.Interfaces;
using API.Services.Interfaces;
using Shared.Enums;

namespace API.Services.BackgroundServices;

public class TicketDailyClosingService : ITicketDailyClosingService
{
    private readonly ITicketRepository _ticketManager;

    public TicketDailyClosingService(
        ITicketRepository ticketManager
    )
    {
        _ticketManager = ticketManager;
    }

    public async Task CloseOldTickets()
    {
        IEnumerable<Ticket> tickets = await _ticketManager.GetPendingTicketsAsync();
        DateTime oneMonthAgo = DateTime.UtcNow.AddMonths(-1);
        
        foreach (var ticket in tickets)
        {
            var lastMessage = ticket.Messages?
                .OrderByDescending(m => m.DateEnvoi)
                .FirstOrDefault();
            DateTime lastActivityDate = lastMessage?.DateEnvoi ?? ticket.DateCreation;
            if (lastActivityDate < oneMonthAgo)
            {
                ticket.Status = (int)StatusTicketEnum.CLOSED;
                await _ticketManager.UpdateAsync(ticket);
            }
        }
    }
}