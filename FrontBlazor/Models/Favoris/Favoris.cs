namespace FrontBlazor.Models;

public class Favoris : IEntity
{
    public int AnnonceId { get; set; }
    public int UtilisateurId { get; set; }

    public int GetId()
    {
        return AnnonceId;
    }
}