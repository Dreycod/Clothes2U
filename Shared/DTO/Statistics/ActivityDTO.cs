using System.Text.Json.Serialization;

namespace Shared.DTO;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(ActivityRestauration), "restauration")]
[JsonDerivedType(typeof(ActivitySignalement), "signalement")]
[JsonDerivedType(typeof(ActivityTicket), "ticket")]
[JsonDerivedType(typeof(ActivityDemandeAnalyse), "analyse")]
public abstract class ActivityDTO
{
    public string LoginUser { get; set; }
    public DateTime Date { get; set; }
}

public class ActivityRestauration : ActivityDTO
{
    public int RestaurationId { get; set; }
}

public class ActivitySignalement : ActivityDTO
{
    public int SignalementId { get; set; }
    public string UtiliseurSignaleLogin { get; set; }
}

public class ActivityTicket : ActivityDTO
{
    public int TicketId { get; set; }
    public string TicketSubject { get; set; }
}

public class ActivityDemandeAnalyse : ActivityDTO
{
    public int AnnonceId { get; set; }
}