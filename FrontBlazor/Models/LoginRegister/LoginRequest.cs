using System.ComponentModel.DataAnnotations;

namespace FrontBlazor.Models.LoginRegister;

public class LoginRequest
{
    [Required(ErrorMessage = "Le login est requis")]
    public string? Login { get; set; }
    [Required(ErrorMessage = "L'email est requis")]
    [EmailAddress(ErrorMessage = "Format d'email invalide")]
    public string? Email { get; set; }
    [Required(ErrorMessage = "Mot de passe obligatoire.")]
    public string Password { get; set; }
    [Required(ErrorMessage = "Veuillez confirmer votre mot de passe")]
    [Compare(nameof(Password), ErrorMessage = "Les mots de passe ne correspondent pas")]
    public string? PasswordConfirm { get; set; }
}

