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
using API.Services;
using AutoMapper;

namespace API.Controllers;

public class LoginRequest
{
    public string? Login { get; set; }
    
    [EmailAddress(ErrorMessage = "Email invalide.")]
    public string? Email { get; set; }
    
    [Required(ErrorMessage = "Mot de passe obligatoire.")]
    [DataType(DataType.Password)]
    [RegularExpression(
        @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
        ErrorMessage = "Mot de passe non conforme."
    )]
    public string Password { get; set; }
    
    public string? PasswordConfirm { get; set; }
}

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IDataRepository<Utilisateur, int> _dataRepository;
    private readonly ILoginService _loginService;
    private List<Utilisateur>? _utilisateurs;
    private readonly IMapper _mapper;

    public LoginController(IConfiguration config, IMapper mapper, IDataRepository<Utilisateur, int> dataRepo, ILoginService loginService)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _mapper = mapper;
        _dataRepository = dataRepo;
        _loginService = loginService;
        
        // LOG POUR VÉRIFIER QUE LA CONFIG EST BIEN CHARGÉE
        Console.WriteLine($"📋 [LoginController] Configuration chargée:");
        Console.WriteLine($"   Jwt:Key = {(_config["Jwt:Key"]?.Length > 0 ? "✅ Présent" : "❌ Absent")}");
        Console.WriteLine($"   Jwt:Issuer = '{_config["Jwt:Issuer"]}'");
        Console.WriteLine($"   Jwt:Audience = '{_config["Jwt:Audience"]}'");
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var utilisateurs = await _dataRepository.GetAllAsync();
        var usersList = utilisateurs?.ToList();
        
        if (string.IsNullOrEmpty(request.Login) && string.IsNullOrEmpty(request.Email))
        {
            return BadRequest("Email ou login obligatoires.");
        }

        var loginOrEmail = string.IsNullOrEmpty(request.Login) ? request.Email : request.Login;
        var auth = _loginService.AuthenticateUtilisateur(loginOrEmail!, request.Password, usersList);;

        if (auth.result != AuthResult.Success)
        {
            if(auth.result == AuthResult.InvalidLoginOrEmail)
                return Unauthorized("Utilisateur inconnu.");
            return Unauthorized("Votre mot de passe est incorrect.");
        }
            

        var utilisateur = auth.user;

        // Génération JWT
        var tokenString = _loginService.GenerateJwtToken(utilisateur);

        // Cookie HttpOnly
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false, 
            SameSite = SameSiteMode.Lax,
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
        if (request == null)
            return BadRequest("Données invalides.");
        
        if (string.IsNullOrEmpty(request.Login))
        {
            return BadRequest("Login obligatoires.");
        }

        if (string.IsNullOrEmpty(request.Email))
        {
            return BadRequest("Email obligatoire.");
        }
        
        
        if (string.IsNullOrEmpty(request.PasswordConfirm))
        {
            return BadRequest("Confirmation de mot de passe obligatoire.");
        }
        
        if (request.Password != request.PasswordConfirm)
            return BadRequest("Les mots de passe ne correspondent pas");

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
        var tokenString = _loginService.GenerateJwtToken(newUser);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = false, 
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
        return Ok("Déconnexion réussie");
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

    // private Utilisateur AuthentificateUtilisateur(string loginOrEmail, string password)
    // {
    //     return _utilisateurs?.SingleOrDefault(u =>
    //         (u.Email.ToUpper() == loginOrEmail.ToUpper() || u.Login.ToUpper() == loginOrEmail.ToUpper())
    //         && BCrypt.Net.BCrypt.Verify(password, u.Password));
    // }

    // private string GenerateJwtToken(Utilisateur utilisateur)
    // {
    //     var key = _config["Jwt:Key"];
    //     var issuer = _config["Jwt:Issuer"];
    //     var audience = _config["Jwt:Audience"];
    //
    //     var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    //     var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
    //
    //     var claims = new[]
    //     {
    //         new Claim(JwtRegisteredClaimNames.Sub, utilisateur.Email),
    //         new Claim("userId", utilisateur.UtilisateurId.ToString()),
    //         new Claim("login", utilisateur.Login),
    //         new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    //     };
    //
    //     var tokenDescriptor = new SecurityTokenDescriptor
    //     {
    //         Subject = new ClaimsIdentity(claims),
    //         Expires = DateTime.UtcNow.AddMinutes(30),
    //         Issuer = issuer,
    //         Audience = audience,
    //         SigningCredentials = credentials
    //     };
    //
    //     var tokenHandler = new JwtSecurityTokenHandler();
    //     var token = tokenHandler.CreateToken(tokenDescriptor);
    //     return tokenHandler.WriteToken(token);
    // }
    
}
