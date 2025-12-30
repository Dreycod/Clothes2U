using Shared.Interfaces;

namespace Shared.DTO.SousCategorie;

public class SousCategoriePostDTO: IEntity
{
    public int SousCategorieId { get; set; }
    public string? LibelleSousCategorie { get; set; }
    public int CategorieId { get; set; }
    public int GetId()
    {
        return SousCategorieId;
    }
}
