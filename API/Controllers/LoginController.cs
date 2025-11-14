using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Models.EntityFramework;
using API.Models.Repository;

namespace API.Controllers;
public class LoginRequest
{
    public string? Login { get; set; }
    public string? Email { get; set; }
    public string Password { get; set; }
}
[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IDataRepository<Utilisateur, int> _dataRepository;
    private List<Utilisateur>? _utilisateurs;

    public LoginController(IConfiguration config, IDataRepository<Utilisateur, int> dataRepo)
    {
        _config = config;
        _dataRepository = dataRepo;
    }

    [HttpGet]
    public async Task<IActionResult> GetUtilisateurs()
    {
        var actionResult = await _dataRepository.GetAllAsync();
        if (actionResult == null)
        {
            return BadRequest();       
        }

        _utilisateurs = actionResult.ToList();
        if (_utilisateurs.Count == 0)
        {
            return BadRequest();
        }
        return Ok(_utilisateurs);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        Console.WriteLine("Login : " + request.Login + "Email : " + request.Email + " Password : " + request.Password);
        
        // Charger les utilisateurs
        await LoadUtilisateursAsync();
        
        IActionResult response = Unauthorized();
        var utilisateur = AuthentificateUtilisateur(
            string.IsNullOrEmpty(request.Login) ? request.Email : request.Login,
            request.Password
        );
        
        
        if (utilisateur != null)
        {
            var tokenString = GenerateJwtToken(utilisateur);
            response = Ok(new
            {
                token = tokenString,
                userDetails = new
                {
                    utilisateur.UtilisateurId,
                    utilisateur.Email,
                    utilisateur.Login,
                    utilisateur.Dateinscription,
                    utilisateur.Description,
                    utilisateur.StatutId,
                    utilisateur.AdresseId
                }
            });
        }
        return response;
    }

    private Utilisateur AuthentificateUtilisateur(string login, string password)
    {
        return _utilisateurs?.SingleOrDefault(x => 
            (x.Email.ToUpper() == login.ToUpper() || x.Login.ToUpper() == login.ToUpper()) && 
            BCrypt.Net.BCrypt.Verify(password, x.Password));
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