using FrontBlazor.Models.Message;
namespace FrontBlazor.Models;
public class Conversation : IEntity
{
    public int ConversationId { get; set; }
    public string? LastMessage { get; set; }
    public DateTime? LastMessageDate { get; set; }
    public string? Vendeur { get; set; }
    public string Acheteur { get; set; }
    public string? Annonce { get; set; }
    public int GetId()
    {
        return ConversationId;
    }
}
    