namespace FrontBlazor.Models;
public class SignalementAvis : IEntity
{
    public int SignalementAvisId { get; set; }
    public int AvisId { get; set; }
    public int Note { get; set; }
    public string Commentaire { get; set; } = null!;
    public string Auteur { get; set; } = null!;
    public int GetId()
    {
        return SignalementAvisId;
    }
}