namespace Shared.DTO;

public class AdresseLivraisonDTO
{
    public int AdresseId { get; set; }
    public string AdresseRue { get; set; } = "";
    public string AdresseVille { get; set; } = "";
    public string AdresseCodePostal { get; set; } = "";
    public string AdressePays { get; set; } = "";
}