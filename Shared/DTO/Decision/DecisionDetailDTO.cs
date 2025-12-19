using System.Text.Json.Serialization;


namespace Shared.DTO.Decision;


[JsonPolymorphic(TypeDiscriminatorPropertyName = "$typeDecision")]
[JsonDerivedType(typeof(DecisionAvertissementCreateDTO), "avertissement")]
[JsonDerivedType(typeof(SanctionSuspensionCreateDTO), "sanctionsuspension")]
[JsonDerivedType(typeof(SanctionBanissementCreateDTO), "sanctionbannissement")]
public abstract class DecisionCreateDTO
{
    public int UtilisateurId { get; set; }
    public int ModerateurId { get; set; }
}
public class DecisionAvertissementCreateDTO : DecisionCreateDTO{}

public abstract class DecisionSanctionCreateDTO : DecisionCreateDTO
{
    public ElementDecisionDTO ElementDecision { get; set; }
}

public class SanctionSuspensionCreateDTO : DecisionSanctionCreateDTO
{
    public DateTime SuspensionDate { get; set; }
}
public class SanctionBanissementCreateDTO : DecisionSanctionCreateDTO{}