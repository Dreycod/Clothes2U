using Shared.Interfaces;

namespace Shared.DTO.Taille;

public class TailleDTO: IEntity
{
    public int TailleId { get; set; }
    public string Libelletaille { get; set; }
    public int CategorieId { get; set; }
    public int? NombreProduits { get; set; }

    public int GetId()
    {
        return TailleId;
    }
}