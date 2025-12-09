using System.Text.Json.Serialization;

namespace FrontBlazor.Models.Notification;
public abstract class Notification
{
    public int NotificationId { get; set; }
    public DateTime DateCreation { get; set; }
    public abstract string LibelleType { get; }
    public bool EstLu { get; set; }
    
    // Constructeur sans paramètres - OBLIGATOIRE
    protected Notification() 
    { 
    }
}

public class NotificationAdmin : Notification
{
    public override string LibelleType => "Administration";
    public string? AdminText { get; set; }
    
    // Constructeur sans paramètres - OBLIGATOIRE
    public NotificationAdmin() 
    { 
    }
}

public class NotificationAvertissement : Notification
{
    public override string LibelleType => "Avertissement";
    public string? MessageAvertissement { get; set; }
    
    // Constructeur sans paramètres - OBLIGATOIRE
    public NotificationAvertissement() 
    { 
    }
}

public class NotificationMessage : Notification
{
    public override string LibelleType => "Message";
    public int? ConversationId { get; set; }
    public string? MessagePreview { get; set; }
    
    // Constructeur sans paramètres - OBLIGATOIRE
    public NotificationMessage() 
    { 
    }
}

public class NotificationModificationAnnonce : Notification
{
    public override string LibelleType => "Modification annonce";
    public int? ModificationAnnonceId { get; set; }
    public string? NomAuteur { get; set; }
    public string? Title { get; set; }
    
    // Constructeur sans paramètres - OBLIGATOIRE
    public NotificationModificationAnnonce() 
    { 
    }
}

public class NotificationNouvelleAnnonce : Notification
{
    public override string LibelleType => "Nouvelle annonce";
    public int? NouvelleAnnonceId { get; set; }
    
    // Constructeur sans paramètres - OBLIGATOIRE
    public NotificationNouvelleAnnonce() 
    { 
    }
}