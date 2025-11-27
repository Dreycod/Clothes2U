namespace FrontBlazor.Models;
public class SignalementAnnonce : IEntity
{
    public int SignalementAnnonceId { get; set; }
    public int AnnonceId { get; set; }
    public string Titre { get; set; } = null!;
    public decimal Prix { get; set; }
    public int GetId()
    {
        return SignalementAnnonceId;
    }
}
