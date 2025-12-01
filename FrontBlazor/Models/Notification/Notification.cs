namespace FrontBlazor.Models;
public class Notification
{
    public int NotificationTypeId { get; set; }
    
    
    //message administrateur
    public String? AdminText { get; set; }
    
    
    //avertissement
    public string? MessageAvertissement  { get; set; } = null!;
    
    //Nouveau message
    public int? MessageId { get; set; }
    
    //modification annonce
    public int? ModificationAnnonceId { get; set; }
    
    //nouvelle annonce
    public int? NouvelleAnnonceId { get; set; }
    
    //type d'annonce 
    public string TypeAnnonce  { get; set; } = null!;

    public int GetId()
    {
        return NotificationTypeId;
    }
}