namespace Shared.DTO.Message;

public class MessageSignalementDTO
{
    public int UserId { get; set; }
    public string content { get; set; }
    public List<int> Photos { get; set; }
}