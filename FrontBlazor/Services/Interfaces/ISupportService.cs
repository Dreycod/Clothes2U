using Shared;
using Shared.DTO.SupportTicket;

namespace FrontBlazor.Services.Interfaces
{
    public interface ISupportService
    {
        Task<APIResponse<SupportTicketCreateDTO>> CreateTicketAsync(SupportTicketCreateDTO dto);
        Task<List<SupportTicketViewDTO>> GetOpenTicketsAsync();
        Task ReplyAsync(SupportTicketReplyDTO dto);
    }
}
