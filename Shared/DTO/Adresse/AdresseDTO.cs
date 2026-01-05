using System.ComponentModel.DataAnnotations;

namespace Shared.DTO;

public class AdresseDTO
{
    public int AdresseId { get; set; }
    public string AdresseRue { get; set; } = "";
    public string AdresseVille { get; set; } = "";
    public string AdresseCodePostal { get; set; } = "";
    public string AdressePays { get; set; } = "France";
    public bool IsDefault { get; set; }
    public int UtilisateurId { get; set; }
}