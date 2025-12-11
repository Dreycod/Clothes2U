namespace FrontBlazor.Models;
public class Signalement
{
    public int SignalementId { get; set; }
    public DateTime SignalementDate { get; set; }
    public string SignalementMotif { get; set; } = null!;
    public string Type { get; set; } = null!;
    public string LoginUtilisateurSignale { get; set; } = null!;
    public int? PhotoProfilUtilisateurId { get; set; } 
}



