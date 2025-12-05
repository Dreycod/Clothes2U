namespace API.DTO.Notification;

public class NotificationDTO
{
    public int NotificationId { get; set; }
    public DateTime DateCreation { get; set; }
    public string LibelleType { get; set; }
    public bool EstLu { get; set; }
    
    //message administrateur
    public String? AdminText { get; set; }
    
    
    //avertissement
    public string? MessageAvertissement  { get; set; } = null!;
    
    //Nouveau message
    public int? ConversationId { get; set; }
    public string? MessagePreview { get; set; }    
    //modification annonce
    public int? ModificationAnnonceId { get; set; }
    public string? NomAuteur { get; set; }
    public string? Title {get; set;}
    
    //nouvelle annonce
    public int? NouvelleAnnonceId { get; set; }
}