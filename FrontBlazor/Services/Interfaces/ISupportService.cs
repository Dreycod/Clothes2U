using Shared.DTO.SupportTicket;

namespace FrontBlazor.Services.Interfaces
{
    public interface ISupportService
    {
        Task CreateTicketAsync(SupportTicketCreateDTO dto);
        Task<List<SupportTicketViewDTO>> GetOpenTicketsAsync();
        Task ReplyAsync(SupportTicketReplyDTO dto);
    }
}
