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

    public LoginController(IConfiguration config,IMapper mapper ,IDataRepository<Utilisateur, int> dataRepo)
    {
        _mapper = mapper;
        _config = config;
        _dataRepository = dataRepo;
    }

    // [HttpGet]
    // public async Task<IActionResult> GetUtilisateurs()
    // {
    //     var actionResult = await _dataRepository.GetAllAsync();
    //     if (actionResult == null)
    //     {
    //         return BadRequest();
    //     }
    //
    //     _utilisateurs = actionResult.ToList();
    //     if (_utilisateurs.Count == 0)
    //     {
    //         return BadRequest();
    //     }
    //     return Ok(_utilisateurs);
    // }

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
    
    [HttpPost("signup")]
    public async Task<IActionResult> SignUp([FromBody] LoginRequest request)
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

        string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$";
        if (!Regex.IsMatch(request.Password, pattern))
        {
            return BadRequest("Le mot de passe doit contenir au moins 8 caractères, une majuscule, une minuscule, un chiffre et un caractère spécial.");
        }

        if (request.Password != request.PasswordConfirm)
        {
            return BadRequest("Les mots de passe ne correspondent pas");
        }

        var existingUsers = await _dataRepository.GetAllAsync();
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
            Dateinscription = DateTime.UtcNow,
            RoleId = 1
        };

        await _dataRepository.AddAsync(newUser);

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

    // DELETE: api/Login/5
    /// <summary>
    /// Supprime un utilisateur existant par son identifiant.
    /// </summary>
    /// <param name="id">L'identifiant de l'utilisateur a supprimer.</param>
    /// <returns>Une reponse HTTP indiquant le resultat de l'operation.</returns>
    /// <response code="204">Le produit a ete supprime avec succes.</response>
    /// <response code="404">Aucun produit n'a ete trouve avec l'identifiant specifie.</response>
    [HttpDelete("{id}")]
    
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUtilisateur(int id)
    {
        ActionResult<Utilisateur?> utilisateurToDelete = await _dataRepository.GetByIdAsync(id);
        if (utilisateurToDelete.Value == null)
        {
            return NotFound();
        }

        await _dataRepository.DeleteAsync(utilisateurToDelete.Value);
        return NoContent();
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateUtilisateur(int id, [FromBody] UtilisateurDTO utilisateur)
    {
        if (id != utilisateur.UtilisateurId)
        {
            return BadRequest();
        }

        ActionResult<Utilisateur?> utilisateurToUpdate = await _dataRepository.GetByIdAsync(id);

        if (utilisateurToUpdate.Value == null)
        {
            return NotFound();
        }
        await _dataRepository.UpdateAsync(utilisateurToUpdate.Value, _mapper.Map<Utilisateur>(utilisateur));
        return NoContent();
    }
}