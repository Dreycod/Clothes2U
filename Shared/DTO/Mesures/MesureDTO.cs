using Shared.Interfaces;


namespace Shared.DTO.Mesures;

public class MesureDTO
{
    public int MesureId { get; set; }
    public int TailleId { get; set; }
    public int CategorieId { get; set; }
    public int GetId() => MesureId;

}

