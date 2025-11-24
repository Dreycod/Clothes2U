namespace FrontBlazor.Models;
public class Message: IEntity
{
    public string? MessageId { get; set; }
    public DateTime? Date { get; set; }
    public bool? Lu { get; set; }
    public string? Contenu { get; set; }
    public string? Utilisateur { get; set; }
    public int GetId()
    {
        return int.TryParse(MessageId, out int id) ? id : 0;
    }
}