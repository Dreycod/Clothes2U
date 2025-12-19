namespace Shared.DTO.Utilisateur;

public class UtilisateurViewDTO : UtilisateurDTO
{
    public bool FolloweddByCurrentUser { get; set; }
    public bool BlockedByCurrentUser { get; set; }
    public DateTime DateInscription { get; set; }
    public string Statut { get; set; }
    public int Abonnements { get; set; }
    public int Abonnes { get; set; }
    public int PhotoProfilId { get; set; }
    public double MoyenneAvis { get; set; }
    public int NombreAvis { get; set; }
    public string RoleUtilisateur { get; set; }
}