using API.Models.EntityFramework;
using API.Models.Repository.Interfaces;

namespace API.Models.Repository.Managers;

public class TicketSupportManager : GenericCRUDManager<SupportTicket>
{
    public TicketSupportManager(Clothes2UDbContext context): base(context){}
}