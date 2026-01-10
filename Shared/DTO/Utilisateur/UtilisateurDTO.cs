namespace Shared.DTO.Utilisateur;

public class UtilisateurDTO
{
    public int UtilisateurId { get; set; }
    public string? Email { get; set; }
    public string? Login { get; set; }
    public DateTime? DateInscription { get; set; }
    public string? Description { get; set; }
    public int? AdresseId { get; set; }
    public bool ValidEmail { get; set; }
    public bool ValidTelephone { get; set; }
    public int? StatutId { get; set; }
    public bool PreferenceNotifMail { get; set; }
    public DateTime? DeletedAt { get; set; }
    public int? DeletedByAdminId { get; set; }

}