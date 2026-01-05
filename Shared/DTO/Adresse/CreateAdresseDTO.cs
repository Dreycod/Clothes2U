namespace Shared.DTO;

public class CreateAdresseDTO
{
    public string AdresseRue { get; set; } = "";
    public string AdresseVille { get; set; } = "";
    public string AdresseCodePostal { get; set; } = "";
    public string AdressePays { get; set; } = "France";
    public int UtilisateurId { get; set; }
}