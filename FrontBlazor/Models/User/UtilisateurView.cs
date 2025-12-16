namespace FrontBlazor.Models;

public class UtilisateurView
{
    public int UtilisateurId { get; set; }
    public bool followeddByCurrentUser { get; set; }
    public bool blockedByCurrentUser { get; set; }
    public string Login { get; set; }
    public DateTime DateInscription { get; set; }
    public string Description { get; set; }
    public bool ValidTelephone { get; set; }
    public bool ValidEmail { get; set; }
    public string Statut { get; set; }
    public int Abonnements { get; set; }
    public int Abonnes { get; set; }
    public int PhotoProfilId { get; set; }
    public double MoyenneAvis { get; set; }
    public int NombreAvis { get; set; }
}