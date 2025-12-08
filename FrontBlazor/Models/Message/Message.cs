using System.Collections.ObjectModel;

namespace FrontBlazor.Models;
public class Message: IEntity
{
    public int MessageId { get; set; }
    public int? SenderId { get; set; }
    public string? SenderName { get; set; }
    public bool? SentbyCurrentUser { get; set; }
    public int? UtilisateurId { get; set; }
    public DateTime? Date { get; set; }
    public ObservableCollection<int> ImagesId { get; set; } = new();
    public bool? Lu { get; set; }
    public string? Content { get; set; }
    public Utilisateur? Utilisateur { get; set; }
    public string? TypeMessage { get; set; }
    public int ConversationId { get; set; }
    
    public int GetId() => MessageId ;
}