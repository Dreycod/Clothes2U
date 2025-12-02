using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Controllers;
using API.Models.EntityFramework;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public interface IAuthService
{
    (AuthResult result, Utilisateur? user) AuthenticateUtilisateur(string loginOrEmail, string password, List<Utilisateur> users);
    string GenerateJwtToken(Utilisateur utilisateur);
}

public class AuthService : IAuthService
{
    private readonly IConfiguration _config;

    public AuthService(IConfiguration config)
    {
        _config = config;
    }

    public (AuthResult result, Utilisateur? user) AuthenticateUtilisateur(
        string loginOrEmail, 
        string password, 
        List<Utilisateur> users)
    {
        var utilisateur = users
            .SingleOrDefault(u =>
                u.Email.ToUpper() == loginOrEmail.ToUpper() ||
                u.Login.ToUpper() == loginOrEmail.ToUpper());

        if (utilisateur == null)
            return (AuthResult.InvalidLoginOrEmail, null);

        if (!BCrypt.Net.BCrypt.Verify(password, utilisateur.Password))
            return (AuthResult.InvalidPassword, null);

        return (AuthResult.Success, utilisateur);
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