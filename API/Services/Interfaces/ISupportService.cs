using API.Models.EntityFramework;
using Shared.DTO.SupportTicket;

namespace API.Services.Interfaces
{
    public interface ISupportService
    {
        Task CreateTicket(SupportTicketCreateDTO supportTicketCreateDTO);
        Task Reply(SupportTicketReplyDTO supportTicketReplyDTO);
    }
}
