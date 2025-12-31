using System.Text.Json.Serialization;

namespace Shared.DTO;




[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(ActivityRestauration), "restauration")]
[JsonDerivedType(typeof(ActivitySignalement), "signalement")]
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