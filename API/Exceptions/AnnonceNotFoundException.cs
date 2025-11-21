namespace API.Exceptions;

public class AnnonceNotFoundException : NotFoundException
{
    public AnnonceNotFoundException(int id) 
        : base($"L'annonce avec l'ID {id} n'a pas été trouvée.") { }
}