namespace API.DTO.Utilisateur;

public class UtilisateurDTO
{
    public int? UtilisateurId { get; set; }
    
    public string? Email { get; set; }
    
    public string? Login { get; set; }
    
    public string? Password { get; set; }
    
    public DateTime? Dateinscription { get; set; }
    
    public string? Description { get; set; }
    
    public int? AdresseId { get; set; }
    
    public int? StatutId { get; set; }
}