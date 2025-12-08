namespace API.DTO.Conversation;

public class ConversationDTO
{
    public int ConversationId { get; set; }
    public string? LastMessage { get; set; }
    public DateTime? LastMessageDate { get; set; }
    public string Interlocuteur { get; set; }
    public int PhotoInterlocuteurId { get; set; }
}