using FrontBlazor.Models.Message;
namespace FrontBlazor.Models;
public class ConversationDetail
{
    public int? ConversationId { get; set; }
    public List<Message> ListMessages { get; set; } = new();
    public string? Vendeur { get; set; }
    public string? Acheteur { get; set; }
    public string? Annonce { get; set; }
    public int? Prix { get; set; }
}