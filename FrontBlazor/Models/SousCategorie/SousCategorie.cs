namespace FrontBlazor.Models;
public class SousCategorie
{
    public int SousCategorieId { get; set; }
    public string? LibelleSousCategorie { get; set; }
    public int GetId()
    {
        return SousCategorieId;
    }

}