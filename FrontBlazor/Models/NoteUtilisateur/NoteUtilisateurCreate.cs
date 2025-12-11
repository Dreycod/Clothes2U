namespace FrontBlazor.Models;

public class NoteUtilisateurCreate: IEntity
{
    public int Note { get; set; }
    public string? Commentaire { get; set; }
    public int NoteurId { get; set; }
    public int NoteId { get; set; }

    public int GetId()
    {
        return NoteId;
    }
}

