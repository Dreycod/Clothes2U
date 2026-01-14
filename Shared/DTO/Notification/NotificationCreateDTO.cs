namespace Shared.DTO.Notification;

public abstract class NotificationCreateDTO
{
    public int UtilisateurId { get; set; }
    public int TypeId { get; set; }
    public int NotificationId { get; set; }
}

public class NotificationMessageCreateDTO : NotificationCreateDTO
{
    public int MessageId { get; set; }
    public string MessagePreview { get; set; }
}

public class NotificationAvertissementCreateDTO : NotificationCreateDTO
{
    public string MessageModerateur { get; set; }
}
public class NotificationCommercialCreateDTO : NotificationCreateDTO
{
    public string CommercialText { get; set; }
    public string CommercialTitle { get; set; }
}
public class NotificationAdminCreateDTO : NotificationCreateDTO
{
    public string AdminText{get;set;}
}

public class NotificationPropositionCreateDTO : NotificationCreateDTO
{
    public int PropositionId { get; set; }
}

public class NotificationNouvelleAnnonceCreateDTO : NotificationCreateDTO
{
    public int AnnonceId { get; set; }
}

public class NotificationModificationAnnonceCreateDTO : NotificationCreateDTO
{
    public int AnnonceId { get; set; }
}
public class NotificationAchatCreateDTO : NotificationCreateDTO
{
    public int AnnonceId { get; set; }
    public string Titre { get; set; }
}