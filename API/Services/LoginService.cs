using API.Models.Entity;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services;

public enum AuthResult
{
    Success,
    InvalidLoginOrEmail,
    InvalidPassword
}

public class LoginService : ILoginService
{
    private readonly IConfiguration _config;
    private readonly IUtilisateurRepository _utilisateurRepository;

    public LoginService(
        IConfiguration config,
        IUtilisateurRepository utilisateurRepository)
    {
        _config = config;
        _utilisateurRepository = utilisateurRepository;
    }

    /// <summary>
    /// AMÉLIORATION 1 : Une seule requête DB au lieu de deux
    /// Avant : GetAllAsync() + GetUtilisateurByLogin()
    /// Après : Une seule requête ciblée
    /// </summary>
    public async Task<(AuthResult result, Utilisateur? user)> AuthenticateUtilisateurAsync(
        string loginOrEmail,
        string password)
    {
        // On cherche directement l'utilisateur par email OU login
        // C'est comme chercher une personne dans l'annuaire : 
        // on ne lit pas tout l'annuaire pour ensuite chercher une deuxième fois !
        var utilisateur = await _utilisateurRepository.GetUtilisateurByEmailOrLogin(loginOrEmail);

        if (utilisateur == null)
            return (AuthResult.InvalidLoginOrEmail, null);

        if (!BCrypt.Net.BCrypt.Verify(password, utilisateur.Password))
            return (AuthResult.InvalidPassword, null);

        return (AuthResult.Success, utilisateur);
    }

    /// <summary>
    /// AMÉLIORATION 2 : Toute la logique d'inscription dans le service
    /// Le contrôleur ne fait que déléguer - comme un réceptionniste qui passe 
    /// la demande au service des inscriptions
    /// </summary>
    public async Task<(bool success, string? errorMessage, Utilisateur? user)> RegisterUtilisateurAsync(
        string email,
        string login,
        string password)
    {
        // Vérification email existant
        var existingUserByEmail = await _utilisateurRepository.GetUtilisateurByEmail(email);
        if (existingUserByEmail != null)
            return (false, "Cet email est déjà utilisé.", null);

        // Vérification login existant
        var existingUserByLogin = await _utilisateurRepository.GetUtilisateurByLogin(login);
        if (existingUserByLogin != null)
            return (false, "Ce login est déjà utilisé.", null);

        // Création de l'utilisateur
        var newUser = new Utilisateur
        {
            Email = email,
            Login = login,
            Password = HashPassword(password),
            Description = "",
            StatutId = 1,
            ValidEmail = false,
            ValidTelephone = false,
            Dateinscription = DateTime.UtcNow,
            RoleId = 1
        };

        await _utilisateurRepository.AddAsync(newUser);

        // Récupération de l'utilisateur complet (avec relations)
        var utilisateurComplet = await _utilisateurRepository.GetUtilisateurByLogin(newUser.Login);

        return (true, null, utilisateurComplet);
    }

    /// <summary>
    /// AMÉLIORATION 3 : Logique Google déplacée dans le service
    /// Toute la complexité de "chercher ou créer" est encapsulée ici
    /// </summary>
    public async Task<Utilisateur> GetOrCreateGoogleUtilisateurAsync(GoogleUserInfo userInfo)
    {
        // Recherche utilisateur existant
        var utilisateur = await _utilisateurRepository.GetUtilisateurByEmail(userInfo.Email);
        if (utilisateur != null)
            return utilisateur;

        // Génération d'un login unique
        // C'est comme choisir un nom d'utilisateur disponible sur un réseau social
        var login = await GenerateUniqueLoginAsync(userInfo.Email.Split('@')[0]);

        // Création du nouvel utilisateur
        utilisateur = new Utilisateur
        {
            Email = userInfo.Email,
            Login = login,
            Password = HashPassword(Guid.NewGuid().ToString()), // Mot de passe aléatoire
            Description = "",
            StatutId = 1,
            ValidEmail = true, // Email déjà validé par Google
            ValidTelephone = false,
            Dateinscription = DateTime.UtcNow,
            RoleId = 1
        };

        await _utilisateurRepository.AddAsync(utilisateur);

        // Récupération de l'utilisateur complet avec les relations
        return await _utilisateurRepository.GetUtilisateurByLogin(login);
    }

    public string GenerateJwtToken(Utilisateur utilisateur)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, utilisateur.Email),
            new Claim("userId", utilisateur.UtilisateurId.ToString()),
            new Claim("login", utilisateur.Login),
            new Claim(ClaimTypes.Role, utilisateur.Role.RoleUtilisateurLibelle),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(30), // ✅ UtcNow au lieu de Now
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string HashPassword(string plainPassword)
    {
        return BCrypt.Net.BCrypt.HashPassword(plainPassword);
    }

    /// <summary>
    /// AMÉLIORATION 4 : Centralisation de la création des cookies
    /// Avant : copié-collé 3 fois dans le contrôleur
    /// Après : une seule source de vérité
    /// </summary>
    public CookieOptions CreateAuthCookieOptions(bool isDevelopment)
    {
        return new CookieOptions
        {
            HttpOnly = true,
            Secure = !isDevelopment,
            SameSite = isDevelopment ? SameSiteMode.Lax : SameSiteMode.None,
            Expires = DateTime.UtcNow.AddMinutes(30),
            Path = "/"
        };
    }

    /// <summary>
    /// Méthode privée pour générer un login unique
    /// Si "john" existe, on essaie "john1", "john2", etc.
    /// </summary>
    private async Task<string> GenerateUniqueLoginAsync(string baseLogin)
    {
        var login = baseLogin;
        int counter = 1;

        while (await _utilisateurRepository.GetUtilisateurByLogin(login) != null)
        {
            login = $"{baseLogin}{counter}";
            counter++;
        }

        return login;
    }
}