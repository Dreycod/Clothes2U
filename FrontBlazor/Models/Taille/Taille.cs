namespace FrontBlazor.Models;
public class Taille
{
    public int TailleId { get; set; }
    public string Libelletaille { get; set; }
    public int GetId()
    {
        return TailleId;
    }

}