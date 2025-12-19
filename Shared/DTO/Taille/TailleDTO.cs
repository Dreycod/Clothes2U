namespace Shared.DTO.Taille;

public class TailleDTO
{
    public int TailleId { get; set; }
    public string Libelletaille { get; set; }
    public int CategorieId { get; set; }
    public int GetId()
    {
        return TailleId;
    }
}