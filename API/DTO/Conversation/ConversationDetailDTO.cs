using API.DTO.Message;
using API.Models.EntityFramework;

namespace API.DTO.Conversation;

public class ConversationDetailDTO
{
    public int? ConversationId { get; set; }
    public List<MessageDTO> ListMessages { get; set; } = new();
    // public string? Vendeur { get; set; }
    // public string? Acheteur { get; set; }
    public string? TitreAnnonce { get; set; }
    public int AnnonceId { get; set; }
    public string? Interlocuteur { get; set; }
    public int PhotoAnnonceId { get; set; }
    public double? Prix { get; set; }
}