using Shared.Interfaces;

namespace Shared.DTO.Couleur;

public class CouleurDTO: IEntity
{
    public int CouleurId { get; set; }
    public string Nom { get; set; } = null!;
    public int? NombreProduits { get; set; }

    public int GetId()
    {
        return CouleurId;
    }
}

public class CreateCouleurDTO
{
    public string Nom { get; set; }
}