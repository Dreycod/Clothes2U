using Shared.Interfaces;

namespace Shared.DTO;

public class GenreDTO : IEntity
{
    public int GenreId { get; set; }
    public string? NomGenre { get; set; }
    public int GetId()
    {
        return GenreId;
    }
}