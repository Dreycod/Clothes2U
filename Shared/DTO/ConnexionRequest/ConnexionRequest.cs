using System.ComponentModel.DataAnnotations;

namespace Shared.DTO.ConnexionRequest;

public class LoginRequest
{
    [Required(ErrorMessage = "Email ou Login obligatoire")]
    public string? Login { get; set; }
    
    [Required(ErrorMessage = "Mot de passe obligatoire.")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}

public class RegisterRequest
{
    [Required(ErrorMessage = "Login obligatoire.")]
    public string? Login { get; set; }
    
    [Required(ErrorMessage = "Email obligatoire.")]
    [EmailAddress(ErrorMessage = "Email invalide.")]
    public string? Email { get; set; }
    
    [Required(ErrorMessage = "Mot de passe obligatoire.")]
    [DataType(DataType.Password)]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Mot de passe non conforme."
    )]
    public string? Password { get; set; }
    
    [Required(ErrorMessage = "Veuillez confirmer votre mot de passe.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Les mots de passe ne correspondent pas.")]
    public string? PasswordConfirm { get; set; }
}