namespace Shared.DTO.Marque;

public class MarqueDetailDTO
{
    public int? MarqueID { get; set; }
    public string NomMarque { get; set; }
    public int? NombreProduits { get; set; }
    public int GetId()
    {
        return MarqueID ?? 0;
    }
}
