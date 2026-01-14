namespace API.Services.Interfaces;

public interface ITicketDailyClosingService
{
    Task CloseOldTickets();
}