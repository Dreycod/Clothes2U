using API.Models.Entity;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shared;
using Shared.DTO;
using Shared.DTO.ConnexionRequest;
using Shared.DTO.Utilisateur;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
namespace API.Controllers;


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
    private readonly INotificationRepository _notificationRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IWebHostEnvironment _env;


    public LoginController(
        IConfiguration config,
        IWebHostEnvironment env,
        IMapper mapper,
        IUtilisateurRepository dataRepo,
        ILoginService loginService,
        ICurrentUserService currentUserService,
        INotificationRepository NotificationRepository,
        IMessageRepository messageRepository)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _env = env;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _utilisateurManager = dataRepo;
        _loginService = loginService;
        _notificationRepository = NotificationRepository;
        _messageRepository = messageRepository;
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
        var auth = _loginService.AuthenticateUtilisateur(loginOrEmail!, request.Password, usersList); ;

        if (auth.result != AuthResult.Success)
        {
            if (auth.result == AuthResult.InvalidLoginOrEmail)
                return Unauthorized("Utilisateur inconnu.");
            return Unauthorized("Votre mot de passe est incorrect.");
        }

        Utilisateur utilisateur = await _utilisateurManager.GetUtilisateurByLogin(auth.user.Login);

        var tokenString = _loginService.GenerateJwtToken(utilisateur);
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !_env.IsDevelopment(),              // ✅ HTTPS en prod
            SameSite = _env.IsDevelopment()
                ? SameSiteMode.Lax
                : SameSiteMode.None,                     // ✅ Cross-domain Azure
            Expires = DateTime.UtcNow.AddMinutes(30),
            Path = "/"
        };

        Response.Cookies.Append("authToken", tokenString, cookieOptions);
        return Ok(utilisateur);
    }

    [HttpPost("signup")]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

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
        var utilisateurComplet = await _utilisateurManager
            .GetUtilisateurByLogin(newUser.Login);
        var tokenString = _loginService.GenerateJwtToken(utilisateurComplet);
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = !_env.IsDevelopment(),              // ✅ HTTPS en prod
            SameSite = _env.IsDevelopment()
                ? SameSiteMode.Lax
                : SameSiteMode.None,                     // ✅ Cross-domain Azure
            Expires = DateTime.UtcNow.AddMinutes(30),
            Path = "/"
        };

        Response.Cookies.Append("authToken", tokenString, cookieOptions);
        return Ok(newUser);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("authToken", new CookieOptions
        {
            Secure = !_env.IsDevelopment(),
            SameSite = _env.IsDevelopment()
        ? SameSiteMode.Lax
        : SameSiteMode.None,
            Path = "/"
        });
        return Ok("Déconnexion réussie");
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<CurrentUtilisateurDTO>> GetCurrentUser()
    {
        int userId = await _currentUserService.GetUserIdOrThrow();
        var utilisateur = await _utilisateurManager.GetByIdAsync(userId);
        if (utilisateur == null)
            return NotFound();

        CurrentUtilisateurDTO utilisateurDTO = _mapper.Map<CurrentUtilisateurDTO>(utilisateur);
        utilisateurDTO.MessagesCount = await _messageRepository.GetMessageCountByUserId(userId);
        utilisateurDTO.NotificationsCount = await _notificationRepository.GetNotificationsUnreadCountByUserId(userId);
        return Ok(utilisateurDTO);
    }

    [HttpPatch("modificationMotDePasse")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO passwordDTO)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return BadRequest(APIResponse<object>.ErrorResponse(string.Join(" ", errors)));
        }

        int userId = await _currentUserService.GetUserIdOrThrow();
        Utilisateur user = await _utilisateurManager.GetByIdAsync((int)userId);

        if (user == null)
        {
            return NotFound(APIResponse<object>.ErrorResponse("Utilisateur introuvable"));
        }

        if (passwordDTO.CurrentPassword == passwordDTO.NewPassword)
        {
            return BadRequest(APIResponse<object>.ErrorResponse("Le nouveau mot de passe doit être différent de l'ancien"));
        }

        if (!BCrypt.Net.BCrypt.Verify(passwordDTO.CurrentPassword, user.Password))
        {
            return Unauthorized(APIResponse<object>.ErrorResponse("Mot de passe actuel incorrect."));
        }

        await _utilisateurManager.UpdatePassword(user, BCrypt.Net.BCrypt.HashPassword(passwordDTO.NewPassword));
        return Ok(APIResponse<object>.SuccessResponse(null));
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
    [HttpGet("google-login")]
    [AllowAnonymous]
    public IActionResult GoogleLogin(string returnUrl = "/")
    {
        var clientId = _config["Authentication:Google:ClientId"];
        var redirectUri = _config["Authentication:Google:RedirectUri"];
        var scope = "openid profile email";

        var googleAuthUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
            $"client_id={clientId}&" +
            $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
            $"response_type=code&" +
            $"scope={Uri.EscapeDataString(scope)}&" +
            $"state={Uri.EscapeDataString(returnUrl)}"; // On passe le returnUrl dans state

        Console.WriteLine($"🚀 Redirection vers Google : {googleAuthUrl}");

        return Redirect(googleAuthUrl);
    }

    [HttpGet("google-callback")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code, [FromQuery] string state)
    {
        Console.WriteLine($"🔥 Google Callback appelé avec code: {code?.Substring(0, 20)}...");

        if (string.IsNullOrEmpty(code))
        {
            Console.WriteLine("❌ Code manquant");
            return Redirect($"{_config["FrontendUrl"]}/login?error=no_code");
        }

        try
        {
            var tokenResponse = await ExchangeCodeForToken(code);
            Console.WriteLine($"✅ Access token obtenu");
            var userInfo = await GetGoogleUserInfo(tokenResponse.AccessToken);
            Console.WriteLine($"✅ User info: {userInfo.Email}");
            var utilisateur = await GetOrCreateUtilisateur(userInfo);
            Console.WriteLine($"✅ Utilisateur: {utilisateur.Login}");
            var jwtToken = _loginService.GenerateJwtToken(utilisateur);
            Console.WriteLine($"✅ JWT généré");
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = !_env.IsDevelopment(),              // ✅ HTTPS en prod
                SameSite = _env.IsDevelopment()
                    ? SameSiteMode.Lax
                    : SameSiteMode.None,                     // ✅ Cross-domain Azure
                Expires = DateTime.UtcNow.AddMinutes(30),
                Path = "/"
            };

            Response.Cookies.Append("authToken", jwtToken, cookieOptions);
            Console.WriteLine($"✅ Cookie authToken créé");
            var returnUrl = string.IsNullOrEmpty(state) ? "/" : state;
            var finalUrl = $"{_config["FrontendUrl"]}{returnUrl}";
            Console.WriteLine($"🔀 Redirection vers: {finalUrl}");
            return Redirect(finalUrl);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur: {ex.Message}");
            return Redirect($"{_config["FrontendUrl"]}/login?error=server_error");
        }
    }
    private async Task<GoogleTokenResponse> ExchangeCodeForToken(string code)
    {
        var clientId = _config["Authentication:Google:ClientId"];
        var clientSecret = _config["Authentication:Google:ClientSecret"];
        var redirectUri = _config["Authentication:Google:RedirectUri"];

        using var httpClient = new HttpClient();
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
    {
        { "code", code },
        { "client_id", clientId },
        { "client_secret", clientSecret },
        { "redirect_uri", redirectUri },
        { "grant_type", "authorization_code" }
    });

        var response = await httpClient.PostAsync("https://oauth2.googleapis.com/token", content);
        var json = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"📝 Token response: {json.Substring(0, Math.Min(100, json.Length))}...");

        return JsonSerializer.Deserialize<GoogleTokenResponse>(json);
    }

    private async Task<GoogleUserInfo> GetGoogleUserInfo(string accessToken)
    {
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
        var json = await response.Content.ReadAsStringAsync();

        return JsonSerializer.Deserialize<GoogleUserInfo>(json);
    }

    private async Task<Utilisateur> GetOrCreateUtilisateur(GoogleUserInfo userInfo)
    {
        // Cherche si un utilisateur existe déjà avec cet email
        var existingUsers = await _utilisateurManager.GetAllAsync();
        Utilisateur utilisateur = await _utilisateurManager.GetUtilisateurByEmail(userInfo.Email);

        if (utilisateur != null)
        {
            return utilisateur;
        }

        // Créer un nouveau utilisateur
        var baseLogin = userInfo.Email.Split('@')[0];
        var login = baseLogin;
        int counter = 1;

        while (existingUsers.Any(u => u.Login.ToUpper() == login.ToUpper()))
        {
            login = $"{baseLogin}{counter}";
            counter++;
        }

        utilisateur = new Utilisateur
        {
            Email = userInfo.Email,
            Login = login,
            Password = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()),
            Description = "",
            StatutId = 1,
            ValidEmail = true,
            ValidTelephone = false,
            Dateinscription = DateTime.UtcNow,
            RoleId = 1
        };

        await _utilisateurManager.AddAsync(utilisateur);
        Console.WriteLine($"✅ Nouvel utilisateur créé: {login}");
        utilisateur = await _utilisateurManager.GetUtilisateurByLogin(login);

        return utilisateur;
    }
}
