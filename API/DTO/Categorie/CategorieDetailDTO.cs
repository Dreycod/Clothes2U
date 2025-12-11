using API.DTO.SousCategorie;

namespace API.DTO.Categorie;
public class CategorieDetailDTO
{
    public int IdCategorie { get; set; }
    public string LibelleCategorie { get; set; } = null!;

    public ICollection<SousCategorieDTO> SousCategories { get; set; } = new List<SousCategorieDTO>();

    public int? NombreProduits { get; set; }
}

