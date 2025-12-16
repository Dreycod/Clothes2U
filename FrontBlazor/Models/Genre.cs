namespace FrontBlazor.Models;

public class Genre: IEntity
{
    public int GenreId { get; set; }
    public string? NomGenre { get; set; }

    public int GetId()
    {
        return GenreId;
    }
}