using API.Models.Entity;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Interfaces;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared;
using Shared.DTO;
using Shared.DTO.ConnexionRequest;
using Shared.DTO.Utilisateur;
using System.Text.Json;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IUtilisateurRepository _utilisateurRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly ILoginService _loginService;
    private readonly IMapper _mapper;
    private readonly INotificationRepository _notificationRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly IWebHostEnvironment _env;

    public LoginController(
        IConfiguration config,
        IWebHostEnvironment env,
        IMapper mapper,
        IUtilisateurRepository utilisateurRepository,
        ILoginService loginService,
        ICurrentUserService currentUserService,
        INotificationRepository notificationRepository,
        IMessageRepository messageRepository)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _env = env;
        _mapper = mapper;
        _currentUserService = currentUserService;
        _utilisateurRepository = utilisateurRepository;
        _loginService = loginService;
        _notificationRepository = notificationRepository;
        _messageRepository = messageRepository;
    }

    /// <summary>
    /// AVANT : 2 requêtes DB + logique métier mélangée
    /// APRÈS : Le contrôleur est un simple coordinateur
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        // Validation simple - le contrôleur vérifie juste les données d'entrée
        if (string.IsNullOrEmpty(request.Login))
            return BadRequest("Email ou login obligatoires.");

        // Délégation au service - toute la logique métier est là-bas
        var (result, utilisateur) = await _loginService.AuthenticateUtilisateurAsync(
            request.Login,
            request.Password);

        // Gestion des erreurs - le contrôleur traduit les résultats métier en réponses HTTP
        if (result != AuthResult.Success)
        {
            return result == AuthResult.InvalidLoginOrEmail
                ? Unauthorized("Utilisateur inconnu.")
                : Unauthorized("Votre mot de passe est incorrect.");
        }

        // Génération du token et du cookie
        SetAuthCookie(_loginService.GenerateJwtToken(utilisateur!));

        return Ok(utilisateur);
    }

    /// <summary>
    /// AVANT : Logique de validation et création dispersée
    /// APRÈS : Délégation propre au service
    /// </summary>
    [HttpPost("signup")]
    [AllowAnonymous]
    public async Task<IActionResult> SignUp([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Le service gère toute la logique métier
        var (success, errorMessage, utilisateur) = await _loginService.RegisterUtilisateurAsync(
            request.Email,
            request.Login,
            request.Password);

        if (!success)
            return BadRequest(errorMessage);

        var login = await Login(new LoginRequest { Login = request.Login, Password = request.Password });

        return Ok(utilisateur);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("authToken", new CookieOptions
        {
            Secure = !_env.IsDevelopment(),
            SameSite = _env.IsDevelopment() ? SameSiteMode.Lax : SameSiteMode.None,
            Path = "/"
        });

        return Ok("Déconnexion réussie");
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<CurrentUtilisateurDTO>> GetCurrentUser()
    {
        int userId = await _currentUserService.GetUserIdOrThrow();
        var utilisateur = await _utilisateurRepository.GetByIdAsync(userId);

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
        Utilisateur user = await _utilisateurRepository.GetByIdAsync(userId);

        if (user == null)
            return NotFound(APIResponse<object>.ErrorResponse("Utilisateur introuvable"));

        if (passwordDTO.CurrentPassword == passwordDTO.NewPassword)
            return BadRequest(APIResponse<object>.ErrorResponse("Le nouveau mot de passe doit être différent de l'ancien"));

        if (!BCrypt.Net.BCrypt.Verify(passwordDTO.CurrentPassword, user.Password))
            return Unauthorized(APIResponse<object>.ErrorResponse("Mot de passe actuel incorrect."));

        await _utilisateurRepository.UpdatePassword(
            user,
            _loginService.HashPassword(passwordDTO.NewPassword));

        return Ok(APIResponse<object>.SuccessResponse(null));
    }

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
            $"state={Uri.EscapeDataString(returnUrl)}";

        return Redirect(googleAuthUrl);
    }

    /// <summary>
    /// AVANT : Logique Google mélangée avec la gestion HTTP
    /// APRÈS : Séparation claire des responsabilités
    /// </summary>
    [HttpGet("google-callback")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleCallback([FromQuery] string code, [FromQuery] string state)
    {
        if (string.IsNullOrEmpty(code))
            return Redirect($"{_config["FrontendUrl"]}/login?error=no_code");

        try
        {
            // Étape 1 : Échanger le code contre un token
            var tokenResponse = await ExchangeCodeForToken(code);

            // Étape 2 : Récupérer les infos utilisateur depuis Google
            var userInfo = await GetGoogleUserInfo(tokenResponse.AccessToken);

            // Étape 3 : Déléguer la logique métier au service
            var utilisateur = await _loginService.GetOrCreateGoogleUtilisateurAsync(userInfo);

            // Étape 4 : Générer le JWT et le cookie
            SetAuthCookie(_loginService.GenerateJwtToken(utilisateur));

            // Étape 5 : Rediriger vers le frontend
            var returnUrl = string.IsNullOrEmpty(state) ? "/" : state;
            return Redirect($"{_config["FrontendUrl"]}{returnUrl}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur Google Auth: {ex.Message}");
            return Redirect($"{_config["FrontendUrl"]}/login?error=server_error");
        }
    }

    /// <summary>
    /// AMÉLIORATION : Méthode helper privée pour éviter la duplication
    /// Avant : Code copié-collé 3 fois
    /// Après : Une seule méthode réutilisable
    /// </summary>
    private void SetAuthCookie(string token)
    {
        var cookieOptions = _loginService.CreateAuthCookieOptions(_env.IsDevelopment());
        Response.Cookies.Append("authToken", token, cookieOptions);
    }

    #region Google OAuth Helpers

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

    #endregion
}