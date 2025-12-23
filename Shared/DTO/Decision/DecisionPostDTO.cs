namespace Shared.DTO.Decision;

public abstract class DecisionPostDTO
{
    public int UtlisateurId { get; set; }
    public DateTime DateDecision { get; set; }
}

public class DecisionAvertissementPostDTO : DecisionPostDTO { }

public abstract class DecisionSanctionPostDTO : DecisionPostDTO
{
    public ElementDecisionDTO ElementDecision { get; set; }
}
public class SanctionBannissementPostDTO : DecisionSanctionPostDTO{}

public class SanctionSuspensionPostDTO : DecisionSanctionPostDTO
{
    public DateTime DateFinSuspension { get; set; }
}