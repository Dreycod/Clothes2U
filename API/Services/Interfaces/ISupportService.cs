using API.Models.EntityFramework;
using Shared.DTO.SupportTicket;

namespace API.Services.Interfaces
{
    public interface ISupportService
    {
        Task CreateTicketAsync(int userId, SupportTicketCreateDTO dto);
        Task<List<SupportTicket>> GetOpenTicketsAsync();
        Task ReplyAsync(int adminId, SupportTicketReplyDTO dto);
        Task<int> GetTicketsCountAsync();
    }
}
