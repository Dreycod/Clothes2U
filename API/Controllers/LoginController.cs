using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using API.Models.EntityFramework;
using API.Models.Repository;
using AutoMapper;

namespace API.Controllers;

public class LoginRequest
{
    public string? Login { get; set; }
    public string? Email { get; set; }
    public string Password { get; set; }
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
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        await LoadUtilisateursAsync();

        var loginOrEmail = string.IsNullOrEmpty(request.Login) ? request.Email : request.Login;
        var utilisateur = AuthentificateUtilisateur(loginOrEmail!, request.Password);

        if (utilisateur == null)
            return Unauthorized("Email/Login ou mot de passe incorrect.");

        // Génération JWT
        var tokenString = GenerateJwtToken(utilisateur);

        // Cookie HttpOnly
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // dev local
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.Now.AddMinutes(30)
        };
        Response.Cookies.Append("authToken", tokenString, cookieOptions);

        // Retour direct de l'utilisateur
        return Ok(utilisateur);
    }

    [HttpPost("signup")]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp([FromBody] LoginRequest request)
    {
        if (request == null ||
            string.IsNullOrEmpty(request.Email) ||
            string.IsNullOrEmpty(request.Login) ||
            string.IsNullOrEmpty(request.Password) ||
            string.IsNullOrEmpty(request.PasswordConfirm))
            return BadRequest("Données invalides.");

        if (!new EmailAddressAttribute().IsValid(request.Email))
            return BadRequest("Email invalide.");

        string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
        if (!Regex.IsMatch(request.Password, pattern))
            return BadRequest("Mot de passe non conforme.");

        if (request.Password != request.PasswordConfirm)
            return BadRequest("Les mots de passe ne correspondent pas.");

        var existingUsers = await _dataRepository.GetAllAsync();
        if (existingUsers.Any(u => u.Email.ToUpper() == request.Email.ToUpper()))
            return BadRequest("Cet email est déjà utilisé.");
        if (existingUsers.Any(u => u.Login.ToUpper() == request.Login.ToUpper()))
            return BadRequest("Ce login est déjà utilisé.");

        var newUser = new Utilisateur
        {
            Email = request.Email,
            Login = request.Login,
            Password = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Description = "",
            StatutId = 1,
            Dateinscription = DateTime.UtcNow,
            RoleId = 1
        };

        await _dataRepository.AddAsync(newUser);

        // JWT
        var tokenString = GenerateJwtToken(newUser);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false, // dev local
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.Now.AddMinutes(30)
        };
        Response.Cookies.Append("authToken", tokenString, cookieOptions);

        return Ok(newUser); // Retour direct
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("authToken");
        return Ok(new { message = "Déconnexion réussie" });
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdStr = User.FindFirst("userId")?.Value;
        if (string.IsNullOrEmpty(userIdStr))
            return Unauthorized();

        var utilisateur = await _dataRepository.GetByIdAsync(int.Parse(userIdStr));
        if (utilisateur == null)
            return NotFound();

        return Ok(utilisateur);
    }

    private Utilisateur AuthentificateUtilisateur(string loginOrEmail, string password)
    {
        return _utilisateurs?.SingleOrDefault(u =>
            (u.Email.ToUpper() == loginOrEmail.ToUpper() || u.Login.ToUpper() == loginOrEmail.ToUpper())
            && BCrypt.Net.BCrypt.Verify(password, u.Password));
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

    private async Task LoadUtilisateursAsync()
    {
        var utilisateurs = await _dataRepository.GetAllAsync();
        _utilisateurs = utilisateurs?.ToList();
    }
}
