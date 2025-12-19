namespace Shared.DTO.Utilisateur;

public class UtilisateurPutDTO
{
    public string? Email { get; set; }
    public string? Telephone { get; set; }
    
    public string? Login { get; set; }
    public string? Description { get; set; }
    public int? AdresseId { get; set; }
    public int? StatutId { get; set; }
    public int? PhotoProfilId { get; set; }
    
}