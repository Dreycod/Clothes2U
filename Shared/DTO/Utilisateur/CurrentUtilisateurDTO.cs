namespace Shared.DTO.Utilisateur;

public class CurrentUtilisateurDTO
{
    public int UtilisateurId { get; set; }
    public string Statut { get; set; }
    public string RoleUtilisateur { get; set; }
    public string Login { get; set; }
    public int NotificationsCount { get; set; }
    public int MessagesCount { get; set; }
}