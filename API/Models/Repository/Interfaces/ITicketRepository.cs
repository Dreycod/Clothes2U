using API.Models.EntityFramework;

namespace API.Models.Repository.Interfaces;

public interface ITicketRepository : IDataRepository<Ticket, int>
{
    Task<IEnumerable<Ticket>> GetOpenTicketsAsync();
    Task<int> GetOpenTicketsCountAsync();
}