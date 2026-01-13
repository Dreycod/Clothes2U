using Shared;
using Shared.DTO.SupportTicket;

namespace FrontBlazor.Services.Interfaces
{
    public interface ISupportService
    {
        Task<APIResponse<SupportTicketCreateDTO>> CreateTicketAsync(SupportTicketCreateDTO dto);
        Task<List<SupportTicketViewDTO>> GetOpenTicketsAsync();
        Task<List<SupportTicketViewDTO>> GetPendingTicketsAsync();
        Task ReplyAsync(SupportTicketReplyDTO dto);
        Task CloseTicket(int id);
        Task<TicketDetailViewDTO> GetTicketById(int id);
    }
}
