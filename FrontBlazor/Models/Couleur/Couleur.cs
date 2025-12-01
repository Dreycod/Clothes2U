namespace FrontBlazor.Models;
public class Couleur: IEntity
{
    public int CouleurId { get; set; }
    public string Nom { get; set; } = null!;
    public int GetId()
    {
        return CouleurId;
    }
}