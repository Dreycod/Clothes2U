using Shared.DTO.SousCategorie;

namespace Shared.DTO.Categorie;

public class CategorieDTO
{
    public int IdCategorie { get; set; }
    public string LibelleCategorie { get; set; } = null!;
    
    public ICollection<SousCategorieDTO> SousCategories { get; set; } = new List<SousCategorieDTO>();
    public int GetId()
    {
        return IdCategorie;
    }
}