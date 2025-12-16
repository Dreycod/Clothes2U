using System.Text.Json.Serialization;

namespace FrontBlazor.Models.Notification;
public abstract class Notification
{
    public int NotificationId { get; set; }
    public DateTime DateCreation { get; set; }
    public abstract string LibelleType { get; }
    public bool EstLu { get; set; }
    protected Notification() 
    { 
    }
}

public class NotificationAdmin : Notification
{
    public override string LibelleType => "Administration";
    public string? AdminText { get; set; }
    public NotificationAdmin() 
    { 
    }
}

public class NotificationAvertissement : Notification
{
    public override string LibelleType => "Avertissement";
    public string? MessageAvertissement { get; set; }
    public NotificationAvertissement() 
    { 
    }
}

public class NotificationMessage : Notification
{
    public override string LibelleType => "Message";
    public int? ConversationId { get; set; }
    public string? MessagePreview { get; set; }
    public NotificationMessage() 
    { 
    }
}

public class NotificationModificationAnnonce : Notification
{
    public override string LibelleType => "Modification annonce";
    public int ModificationAnnonceId { get; set; }
    public string NomAuteur { get; set; }
    public string Title { get; set; }
    public NotificationModificationAnnonce() 
    { 
    }
}

public class NotificationNouvelleAnnonce : Notification
{
    public override string LibelleType => "Nouvelle annonce";
    public int NouvelleAnnonceId { get; set; }
    public string NomAuteur { get; set; }
    public NotificationNouvelleAnnonce() 
    { 
    }
}