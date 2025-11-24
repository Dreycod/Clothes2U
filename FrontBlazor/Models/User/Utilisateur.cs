namespace FrontBlazor.Models;

public class Utilisateur : IEntity
{
    public string? Login { get; set; }
    public string? Email { get; set; }
    public string Password { get; set; }
    public string? PasswordConfirm { get; set; }

    public int GetId()
    {
        return 0;
    }
}
