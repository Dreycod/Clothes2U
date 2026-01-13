namespace Shared.DTO.SupportTicket;

public class SupportTicketViewDTO
{
    public int TicketId { get; set; }
    public string Title { get; set; }
    public DateTime DateLastMessage { get; set; }
    public string LoginUser { get; set; }
}