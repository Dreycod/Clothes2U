using System.Text.Json.Serialization;

namespace Shared.DTO.Notification;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$typeNotif")]
[JsonDerivedType(typeof(NotificationAdminDTO), "Administration")]
[JsonDerivedType(typeof(NotificationAvertissementDTO), "Avertissement")]
[JsonDerivedType(typeof(NotificationMessageDTO), "Message")]
[JsonDerivedType(typeof(NotificationModificationAnnonceDTO), "Modification annonce")]
[JsonDerivedType(typeof(NotificationNouvelleAnnonceDTO), "Nouvelle annonce")]
public abstract class NotificationDTO
{
    public int NotificationId { get; set; }
    public DateTime DateCreation { get; set; }
    public bool EstLu { get; set; }
}

public class NotificationAdminDTO : NotificationDTO
{
    public String AdminText { get; set; }
}

public class NotificationAvertissementDTO : NotificationDTO
{
    public string MessageAvertissement  { get; set; } = null!;
}
public class NotificationMessageDTO  : NotificationDTO
{
    public int ConversationId { get; set; }
    public string MessagePreview { get; set; } 
}

public class NotificationModificationAnnonceDTO : NotificationDTO
{
    public int ModificationAnnonceId { get; set; }
    public string NomAuteur { get; set; }
    public string Title {get; set;}
}

public class NotificationNouvelleAnnonceDTO : NotificationDTO
{
    public string NomAuteur { get; set; }
    public int? NouvelleAnnonceId { get; set; }
}