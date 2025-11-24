namespace API.DTO.Conversation;

public class ConversationDTO
{
    public int ConversationId { get; set; }
    public string? LastMessage { get; set; }
    public DateTime? LastMessageDate { get; set; }
    public string? Vendeur { get; set; }
    public string Acheteur { get; set; }
    public string? Annonce { get; set; }
}