namespace FrontBlazor.Models;
public class Signalement: IEntity
{
    public int SignalementId { get; set; }
    public DateTime SignalementDate { get; set; }
    public string SignalementMotif { get; set; } = null!;
    public int SignalementTypeId { get; set; }
    public string Type { get; set; } = null!;
    public int UtilisateurId { get; set; }
    public string LoginAuteur { get; set; } = null!;
    public int GetId()
    {
        return SignalementId;
    }
}
