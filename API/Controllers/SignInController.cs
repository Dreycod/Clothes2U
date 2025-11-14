using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using API.Models.EntityFramework;
using API.Models.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using BCrypt.Net;

namespace API.Controllers;

public class SignInRequest
{
    public string Email { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string? PasswordConfirm { get; set; }
}

[Route("api/[controller]")]
[ApiController]
public class SignInController : ControllerBase
{
    private readonly IDataRepository<Utilisateur, int> _utilisateurRepo;
    private readonly IConfiguration _config;

    public SignInController(IConfiguration config, IDataRepository<Utilisateur, int> utilisateurRepo)
    {
        _config = config;
        _utilisateurRepo = utilisateurRepo;
    }

    [HttpPost]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Email) ||
            string.IsNullOrEmpty(request.Login) || string.IsNullOrEmpty(request.Password) || string.IsNullOrEmpty(request.PasswordConfirm))
        {
            return BadRequest("Données invalides.");
        }
        
        if (!new EmailAddressAttribute().IsValid(request.Email))
        {
            return BadRequest("Email invalide.");
        }
        
        // Au moins 8 caractères, une majuscule, une minuscule, un chiffre et un caractère spécial
        string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
        if (!Regex.IsMatch(request.Password, pattern))
        {
            return BadRequest("Le mot de passe doit contenir au moins 8 caractères, une majuscule, une minuscule, un chiffre et un caractère spécial.");
        }

        if (request.Password != request.PasswordConfirm)
        {
            return BadRequest("Les mots de passe ne correspondent pas");
        }

        var existingUsers = await _utilisateurRepo.GetAllAsync();
        if (existingUsers.Any(u => u.Email.ToUpper() == request.Email.ToUpper()))
            return BadRequest("Cet email est déjà utilisé.");

        if (existingUsers.Any(u => u.Login.ToUpper() == request.Login.ToUpper()))
            return BadRequest("Ce login est déjà utilisé.");

        // Créer le nouvel utilisateur
        var newUser = new Utilisateur
        {
            Email = request.Email,
            Login = request.Login,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Description = "",
            StatutId = 1,
            Dateinscription = DateTime.UtcNow
        };

        await _utilisateurRepo.AddAsync(newUser);

        var tokenString = GenerateJwtToken(newUser);

        return Ok(new
        {
            message = "Inscription réussie.",
            token = tokenString,
            userDetails = new
            {
                newUser.UtilisateurId,
                newUser.Email,
                newUser.Login,
                newUser.Dateinscription
            }
        });
    }

    private string GenerateJwtToken(Utilisateur utilisateur)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, utilisateur.Email),
            new Claim("userId", utilisateur.UtilisateurId.ToString()),
            new Claim("login", utilisateur.Login),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}