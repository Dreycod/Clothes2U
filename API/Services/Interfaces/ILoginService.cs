using API.Models.Entity;
using API.Models.EntityFramework;
using Microsoft.AspNetCore.Http;

namespace API.Services;

public interface ILoginService
{
    /// <summary>
    /// Authentifie un utilisateur et retourne l'utilisateur complet si succès
    /// </summary>
    Task<(AuthResult result, Utilisateur? user)> AuthenticateUtilisateurAsync(
        string loginOrEmail,
        string password);

    /// <summary>
    /// Inscrit un nouvel utilisateur et retourne l'utilisateur créé
    /// </summary>
    Task<(bool success, string? errorMessage, Utilisateur? user)> RegisterUtilisateurAsync(
        string email,
        string login,
        string password);

    /// <summary>
    /// Authentification Google : récupère ou crée un utilisateur depuis les infos Google
    /// </summary>
    Task<Utilisateur> GetOrCreateGoogleUtilisateurAsync(GoogleUserInfo userInfo);

    /// <summary>
    /// Génère un token JWT pour un utilisateur
    /// </summary>
    string GenerateJwtToken(Utilisateur utilisateur);

    /// <summary>
    /// Hash un mot de passe
    /// </summary>
    string HashPassword(string plainPassword);

    /// <summary>
    /// Crée les options de cookie appropriées selon l'environnement
    /// </summary>
    CookieOptions CreateAuthCookieOptions(bool isDevelopment);
}