namespace FrontBlazor.Models;
public class ConversationDetail : IEntity
{
    public int ConversationId { get; set; }
    public List<Message> ListMessages { get; set; } = new();
    // public string? Vendeur { get; set; }
    // public string? Acheteur { get; set; }
    public string? TitreAnnonce { get; set; }
    public int AnnonceId { get; set; }
    public string? Interlocuteur { get; set; }
    public int PhotoAnnonceId { get; set; }
    public double? Prix { get; set; }
    
    public int GetId()
    {
        return ConversationId;
    }
}