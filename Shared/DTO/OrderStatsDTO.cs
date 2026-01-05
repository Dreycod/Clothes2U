namespace Shared.DTO;

public class OrderStatsDTO
{
    public int TotalCommandes { get; set; }
    public int CommandesEnCours { get; set; }
    public int CommandesLivrees { get; set; }
    public decimal MontantTotal { get; set; }
}