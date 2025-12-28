using System.Text.Json.Serialization;

namespace Shared.DTO.Decision;




[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(DecisionSuspensionDetailDTO), "decisionSuspension")]
[JsonDerivedType(typeof(DecisionBannissementDetailDTO), "decisionBannissement")]
public abstract class DecisionDetailDTO
{
    public DateTime DateSanction { get; set; } 
    public ElementDecisionDTO ElementDecision { get; set; }
}

public class DecisionSuspensionDetailDTO : DecisionDetailDTO
{
    public DateTime DateFinSuspension { get; set; }
} 
public class DecisionBannissementDetailDTO : DecisionDetailDTO{}