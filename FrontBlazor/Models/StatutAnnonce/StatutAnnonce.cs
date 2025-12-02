namespace FrontBlazor.Models;
public class StatutAnnonce : IEntity
{
    public int StatutAnnonceId { get; set; }
    public string? StatutLibelle { get; set; }
    public int GetId()
    {
        return StatutAnnonceId;
    }

}
