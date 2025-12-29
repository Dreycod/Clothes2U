namespace Shared.DTO.DemandeRestauration;

public class DecisionDemandeRestaurationDTO
{
    public int UtilisateurId { get; set; }
    public int DemandeId { get; set; }
    public bool IsRestored { get; set; }
}