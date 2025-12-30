using Shared.DTO.SousCategorie;
using Shared.Interfaces;

namespace Shared.DTO.Categorie;

public class CategorieDTO: IEntity
{
    public int IdCategorie { get; set; }
    public string LibelleCategorie { get; set; } = null!;
    public int? NombreProduits { get; set; }

    public ICollection<SousCategorieDTO> SousCategories { get; set; } = new List<SousCategorieDTO>();
    public int GetId()
    {
        return IdCategorie;
    }
}