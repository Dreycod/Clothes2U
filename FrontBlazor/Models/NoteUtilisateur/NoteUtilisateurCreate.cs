namespace FrontBlazor.Models;

public class NoteUtilisateurCreate: IEntity
{
    public int Note { get; set; }
    public string? Commentaire { get; set; }
    public int CibleId { get; set; }

    public int GetId()
    {
        return CibleId;
    }
}

