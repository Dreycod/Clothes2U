using System.Collections.ObjectModel;

namespace FrontBlazor.Models;
public class Conversation : IEntity
{
    public int ConversationId { get; set; }
    public string? LastMessage { get; set; }
    public DateTime? LastMessageDate { get; set; }
    public string? Interlocuteur { get; set; }
    public int? PhotoInterlocuteurId { get; set; }
    public List<Message>? ListMessages { get; set; } = new();
    public string? TitreAnnonce { get; set; }
    public int? AnnonceId { get; set; }
    public int? PhotoAnnonceId { get; set; }
    public double? Prix { get; set; }
    public int GetId()
    {
        return ConversationId;
    }
}
    