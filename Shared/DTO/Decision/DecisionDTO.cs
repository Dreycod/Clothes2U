namespace Shared.DTO.Decision;

public class DecisionDTO
{
    public string TypeDecision { get; set; }
    public string LoginUtilisateur { get; set; }
    public DateTime DateDecision { get; set; }
    public bool? Statut { get; set; }
    public DateTime? FinSuspension { get; set; }
}