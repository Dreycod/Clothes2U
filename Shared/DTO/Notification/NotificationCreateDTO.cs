namespace Shared.DTO.Notification;

public abstract class NotificationCreateDTO
{
    public int TypeId { get; set; }
}

public class NotificationMessageCreateDTO : NotificationCreateDTO
{
    
    public int UtilisateurId { get; set; }
    public int MessageId { get; set; }
    public string MessagePreview { get; set; }
}

public class NotificationAvertissementCreateDTO : NotificationCreateDTO
{
    public int UtilisateurId { get; set; }
    public string MessageModerateur { get; set; }
}
public class NotificationCommercialCreateDTO : NotificationCreateDTO
{
    public string CommercialText { get; set; }
    public string CommercialTitle { get; set; }
}
public class NotificationPropositionCreateDTO : NotificationCreateDTO
{
    public int UtilisateurId { get; set; }
    public int PropositionId { get; set; }
}

public class NotificationNouvelleAnnonceCreateDTO : NotificationCreateDTO
{
    public int UtilisateurIdFollowed { get; set; }
    public string AnnonceTitle { get; set; }
    public string UtilisateurLogin { get; set; }
    public int AnnonceId { get; set; }
}

public class NotificationModificationAnnonceCreateDTO : NotificationCreateDTO
{
    public int AnnonceId { get; set; }
    public string AnnonceTitle { get; set; }
}
public class NotificationAchatCreateDTO : NotificationCreateDTO
{
    public int AnnonceId { get; set; }
    public string Titre { get; set; }
}