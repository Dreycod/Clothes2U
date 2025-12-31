using Shared.Interfaces;
using Shared.DTO.Mesures;
namespace Shared.DTO.Taille;

public class TailleDTO: IEntity
{
    public int TailleId { get; set; }
    public string Libelletaille { get; set; }
    //public int CategorieId { get; set; }
    public int? NombreProduits { get; set; }
    public ICollection<MesureDTO> Mesures { get; set; } = new List<MesureDTO>();

    public int GetId()
    {
        return TailleId;
    }
}