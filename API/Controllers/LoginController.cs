using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using API.DTO.Utilisateur;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services;
using AutoMapper;

namespace API.Controllers;

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

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IUtilisateurRepository _utilisateurManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILoginService _loginService;
    private List<Utilisateur>? _utilisateurs;
    private readonly IMapper _mapper;

    public LoginController(IConfiguration config, IMapper mapper, IUtilisateurRepository dataRepo, ILoginService loginService, ICurrentUserService currentUserService)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _mapper = mapper;
        _currentUserService = currentUserService;
        _utilisateurManager = dataRepo;
        _loginService = loginService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var utilisateurs = await _utilisateurManager.GetAllAsync();
        var usersList = utilisateurs?.ToList();
        
        if (string.IsNullOrEmpty(request.Login))
        {
            return BadRequest("Email ou login obligatoires.");
        }

        var loginOrEmail = request.Login;
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
    public async Task<IActionResult> SignUp([FromBody] RegisterRequest request)
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

        var existingUsers = await _utilisateurManager.GetAllAsync();
        
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
            ValidEmail = false,
            ValidTelephone = false,
            Dateinscription = DateTime.UtcNow,
            RoleId = 1
        };

        await _utilisateurManager.AddAsync(newUser);

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
        int? userId = await _currentUserService.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }
        var utilisateur = await _utilisateurManager.GetByIdAsync((int)userId);
        if (utilisateur == null)
            return NotFound();
        
        UtilisateurViewDTO utilisateurDTO = _mapper.Map<UtilisateurViewDTO>(utilisateur);
        return Ok(utilisateurDTO);
    }

    [HttpPut("modificationMotDePasse")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword(
        [FromQuery] string currentPassword,
        [FromQuery] string newPassword,
        [FromQuery] string confirmNewPassword)
    {
        int? userId = await _currentUserService.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }

        if (newPassword != confirmNewPassword)
        {
            return BadRequest("Les nouveaux mots de passe ne correspondent pas.");
        }

        Utilisateur user = await _utilisateurManager.GetByIdAsync((int)userId);
    
        if (user == null)
        {
            return NotFound();
        }
        if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))
        {
            return Unauthorized("Mot de passe actuel incorrect.");
        }
    
        await _utilisateurManager.UpdatePassword(user, BCrypt.Net.BCrypt.HashPassword(newPassword));
        return NoContent();
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
