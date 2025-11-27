namespace FrontBlazor.Models;
public class Categorie : IEntity
{
    public int IdCategorie { get; set; }
    public string LibelleCategorie { get; set; } = null!;
    
    public ICollection<SousCategorie> SousCategories { get; set; } = new List<SousCategorie>();
    public int GetId()
    {
        return IdCategorie;
    }

}