namespace FrontBlazor.Models;

public class UtilisateurView
{
    public int UtilisateurId { get; set; }
    public string Login { get; set; }
    public DateTime DateInscription { get; set; }
    public string Description { get; set; }
    public string Statut { get; set; }
    public int Abonnements { get; set; }
    public int Abonnes { get; set; }
    public double Moyenne { get; set; }
    public int PhotoProfilId { get; set; }

}