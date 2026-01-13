using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public class TicketMessageManager : GenericCRUDManager<TicketMessage>
{
    public  TicketMessageManager(Clothes2UDbContext context) : base(context){}
}