namespace FrontBlazor.Models;
public class SignalementDetail : IEntity
{
    public int SignalementId { get; set; }
    public DateTime SignalementDate { get; set; }
    public string SignalementMotif { get; set; } = null!;
    public string Type { get; set; } = null!;

    public int UtilisateurId { get; set; }
    public string LoginAuteur { get; set; } = null!;

    public SignalementAnnonce? Annonce { get; set; }
    public SignalementAvis? Avis { get; set; }
    public SignalementUtilisateur? Utilisateur { get; set; }
    public int GetId()
    {
        return SignalementId;
    }
}

