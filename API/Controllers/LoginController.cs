using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;

namespace API.Controllers;

public class LoginRequest
{
    public string? Login { get; set; }
    public string? Email { get; set; }
    public string Password { get; set; } = string.Empty;
    public string? PasswordConfirm { get; set; }
}

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IDataRepository<Utilisateur, int> _dataRepository;
    private List<Utilisateur>? _utilisateurs;
    private readonly IMapper _mapper;

    public LoginController(IConfiguration config, IMapper mapper, IDataRepository<Utilisateur, int> dataRepo)
    {
        _mapper = mapper;
        _config = config;
        _dataRepository = dataRepo;
        
        // 💡 Log de diagnostic de la clé de signature au démarrage
        string key = _config["Jwt:Key"];
        Console.WriteLine($"[LoginController Init] 🔑 Clé JWT lue (longueur {key.Length}): {key.Substring(0, 10)}...");
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        await LoadUtilisateursAsync();
        Console.WriteLine($"[LoginController] ➡️ Tentative de connexion pour : {request.Login ?? request.Email}");
        
        string? loginOrEmail = request.Login?.Trim();
        if (string.IsNullOrEmpty(loginOrEmail))
            loginOrEmail = request.Email?.Trim();

        if (string.IsNullOrEmpty(loginOrEmail) || string.IsNullOrEmpty(request.Password))
            return BadRequest("Email/Login et mot de passe obligatoires.");

        var utilisateur = AuthentificateUtilisateur(loginOrEmail, request.Password);
        if (utilisateur == null)
        {
            Console.WriteLine("[LoginController] ❌ Échec de l'authentification.");
            return Unauthorized("Email/Login ou mot de passe incorrect.");
        }
        
        Console.WriteLine($"[LoginController] ✅ Authentification réussie pour UtilisateurId: {utilisateur.UtilisateurId}");

        var tokenString = GenerateJwtToken(utilisateur);
        Console.WriteLine($"[LoginController] 🔑 JWT généré pour utilisateur {utilisateur.UtilisateurId}");

// ✅ Configuration du cookie sécurisé
        CookieOptions cookieOptions = new CookieOptions()
        {
            HttpOnly = true,              // Protection contre XSS
            SameSite = SameSiteMode.None, // Nécessaire pour cross-origin
            Secure = true,                // Obligatoire avec SameSite=None
            Expires = DateTimeOffset.UtcNow.AddHours(1), // ✅ Cohérent avec l'expiration du JWT
            Path = "/",
            Domain = null                 // ✅ Laissez le domaine se définir automatiquement
        };

        Response.Cookies.Append("authToken", tokenString, cookieOptions);
        Console.WriteLine("[LoginController] 🍪 Cookie 'authToken' ajouté (Secure=True, SameSite=None, HttpOnly=True)");

// Retourner le token dans la réponse JSON
        return Ok(new
        {
            utilisateur = new 
            {
                utilisateur.UtilisateurId,
                utilisateur.Login,
                utilisateur.Email
                // N'incluez PAS le mot de passe !
            },
            token = tokenString
        });
    }

    // ... (méthode SignUp)

   [HttpGet("me")]
public async Task<IActionResult> GetCurrentUser()
{
    Console.WriteLine("[LoginController] ➡️ Entrée dans GetCurrentUser");
    
    // ✅ CORRECTION : Utiliser "uid" au lieu de "userId"
    var claim = User.FindFirst("uid")?.Value;
    
    if(string.IsNullOrEmpty(claim))
    {
        Console.WriteLine("[LoginController] ❌ Claim 'uid' manquant dans le token.");
        Console.WriteLine("[LoginController] Claims disponibles:");
        foreach(var c in User.Claims)
        {
            Console.WriteLine($"   - {c.Type} = {c.Value}");
        }
        return Unauthorized();
    }
    
    int userId = int.Parse(claim);
    Console.WriteLine($"[LoginController] 🔍 Utilisateur ID extrait du token: {userId}");
    
    Utilisateur user = await _dataRepository.GetByIdAsync(userId); 
    
    if(user == null)
    {
        Console.WriteLine($"[LoginController] ❌ Utilisateur {userId} non trouvé en base de données.");
        return NotFound();
    }
    
    Console.WriteLine($"[LoginController] ✅ Utilisateur courant récupéré : {user.Login}");
    return Ok(user);
}

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("authToken");
        return Ok(new { message = "Déconnexion réussie" });
    }

    private Utilisateur? AuthentificateUtilisateur(string loginOrEmail, string password)
    {
        return _utilisateurs?.SingleOrDefault(u =>
            (u.Email?.ToUpper() == loginOrEmail.ToUpper() || u.Login?.ToUpper() == loginOrEmail.ToUpper())
            && BCrypt.Net.BCrypt.Verify(password, u.Password));
    }

    private string GenerateJwtToken(Utilisateur utilisateur)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // ✅ SOLUTION : Réduire le nombre de claims et utiliser des noms courts
        var claims = new[]
        {
            new Claim("uid", utilisateur.UtilisateurId.ToString()),  // ✅ "uid" au lieu de "userId"
            new Claim("role", "Authorized"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),  // ✅ Cohérent avec le cookie
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
    
        // ✅ Log pour vérifier la longueur du token
        Console.WriteLine($"[JWT] Token généré - Longueur: {tokenString.Length} caractères");
        Console.WriteLine($"[JWT] Token: {tokenString}"); // Pour debug uniquement, à retirer en production
    
        return tokenString;
    }

    private async Task LoadUtilisateursAsync()
    {
        var utilisateurs = await _dataRepository.GetAllAsync();
        _utilisateurs = utilisateurs?.ToList();
    }
}