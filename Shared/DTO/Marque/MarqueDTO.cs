using Shared.Interfaces;

namespace Shared.DTO.Marque;

public class MarqueDTO : IEntity
{
    public int MarqueID { get; set; }
    public string NomMarque { get; set; }
    public int GetId()
    {
        return MarqueID;
    }
}