namespace API.Exceptions;

public class PhotoNotFoundException : NotFoundException
{
    public PhotoNotFoundException(string fileName) 
        : base($"La photo '{fileName}' n'a pas été trouvée.") { }
}