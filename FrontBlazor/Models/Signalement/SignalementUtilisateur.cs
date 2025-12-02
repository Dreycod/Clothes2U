namespace FrontBlazor.Models;
public class SignalementUtilisateur : IEntity
{
    public int SignalementUtilisateurId { get; set; }
    public int UtilisateurSignaleId { get; set; }
    public string Login { get; set; } = null!;
    public int GetId()
    {
        return SignalementUtilisateurId;
    }
}

