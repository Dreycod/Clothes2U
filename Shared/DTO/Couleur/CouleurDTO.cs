namespace Shared.DTO.Couleur;

public class CouleurDTO
{
    public int CouleurId { get; set; }
    public string Nom { get; set; } = null!;
    public int GetId()
    {
        return CouleurId;
    }
}