using API.Models.EntityFramework;
using API.Models.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;

namespace API.Models.Repository.Managers;

public class TicketManager : GenericCRUDManager<Ticket>, ITicketRepository
{
    public TicketManager(Clothes2UDbContext context): base(context){}

    public async Task<IEnumerable<Ticket>> GetOpenTicketsAsync()
    {
        return BaseTicketQuery().Where(t => t.Status == (int)StatusTicketEnum.OPEN);
    }
    private IQueryable<Ticket> BaseTicketQuery()
    {
        return _context.Tickets
            .Include(t => t.Utilisateur)
            .Include(t => t.Messages)
            .ThenInclude(m => m.Utilisateur)
            .AsSplitQuery();
    }
    public async Task<int> GetOpenTicketsCountAsync()
    {
        return BaseTicketQuery().Where(t => t.Status == (int)StatusTicketEnum.OPEN).Count();
    }
    public async Task<IEnumerable<Ticket>> GetPendingTicketsAsync()
    {
        return BaseTicketQuery().Where(t => t.Status == (int)StatusTicketEnum.ANSWERED);
    }
    public async override Task<Ticket?> GetByIdAsync(int id)
    {
        return await BaseTicketQuery().FirstOrDefaultAsync(t => t.TicketId == id);
    }
}