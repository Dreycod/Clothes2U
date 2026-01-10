using System.ComponentModel.DataAnnotations;

namespace Shared.DTO.LoginRegister;

public class LoginRequestDTO
{
    [Required(ErrorMessage = "Email ou Login obligatoire")]
    public string? Login { get; set; }

    [Required(ErrorMessage = "Mot de passe obligatoire.")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }
}

public class RegisterRequestDTO
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
        ErrorMessage = "Le mot de passe doit contenir 8 caractères dont une majuscule, une minuscule, un chiffre et un caractère spécial"
    )]
    public string? Password { get; set; }

    [Required(ErrorMessage = "Veuillez confirmer votre mot de passe.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Les mots de passe ne correspondent pas.")]
    public string? PasswordConfirm { get; set; }
}

