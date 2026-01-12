using System.Text.Json.Serialization;

namespace Shared.DTO.Decision;


[JsonPolymorphic(TypeDiscriminatorPropertyName = "$typeDecision")]
[JsonDerivedType(typeof(DecisionAvertissementPostDTO), "avertissement")]
[JsonDerivedType(typeof(SanctionSuspensionPostDTO), "sanctionsuspension")]
[JsonDerivedType(typeof(SanctionBannissementPostDTO), "sanctionbannissement")]
[JsonDerivedType(typeof(DecisionIgnorPostDTO), "Ignore")]
public abstract class DecisionPostDTO
{
    public ElementDecisionDTO ElementDecision { get; set; }
    public int UtilisateurId { get; set; }
    public int SignalementId { get; set; }
}

public class DecisionAvertissementPostDTO : DecisionPostDTO
{
    public string MessageModerateur { get; set; }
}

public abstract class DecisionSanctionPostDTO : DecisionPostDTO { }
public class SanctionBannissementPostDTO : DecisionSanctionPostDTO{}

public class SanctionSuspensionPostDTO : DecisionSanctionPostDTO
{
    public DateTime DateFinSuspension { get; set; }
}
public class DecisionIgnorPostDTO : DecisionPostDTO{}