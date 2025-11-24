namespace API.DTO.Message;

public class MessageDTO
{
    public string? MessageId { get; set; }
    public DateTime? Date { get; set; }
    public bool? Lu { get; set; }
    public string? Contenu { get; set; }
    public string? Utilisateur { get; set; }
}