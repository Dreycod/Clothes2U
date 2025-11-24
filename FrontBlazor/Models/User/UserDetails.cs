namespace FrontBlazor.Models;

public class UserDetails: IEntity
{
    public int UtilisateurId { get; set; }
    public string Email { get; set; }
    public string Login { get; set; }
    public DateTime Dateinscription { get; set; }
    public string? Description { get; set; }
    public int? StatutId { get; set; }
    public int? AdresseId { get; set; }
    public int GetId()
    {
        return UtilisateurId;
    }
}
