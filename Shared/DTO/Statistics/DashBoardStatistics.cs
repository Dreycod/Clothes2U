namespace Shared.DTO;

public class DashBoardStatistics
{
    public int SignalementsEnAttented { get; set; }
    public int CompteSuspendus { get; set; }
    public int ResaurationEnAttente { get; set; }
    //public int DemandeSupport { get; set; }
    
    public DecisionStatistics DecisionsAujourdhui { get; set; }
    public DecisionStatistics DecisionsSemaine  { get; set; }
    public DecisionStatistics DecisionsMois { get; set; }
}