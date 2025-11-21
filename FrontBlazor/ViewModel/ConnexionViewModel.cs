using FrontBlazor.Models.LoginRegister;
using FrontBlazor.Services;
using Microsoft.JSInterop;

namespace FrontBlazor.ViewModel;

public class ConnexionViewModel
{
    public string? LoginEmail { get; set; } = null;
    public string? LoginPassword { get; set; } = null;
    public bool ShowLoginPassword { get; set; } = false;

    public string RegisterUsername { get; set; } = string.Empty;
    public string RegisterEmail { get; set; } = string.Empty;
    public string RegisterPassword { get; set; } = string.Empty;
    public string RegisterConfirmPassword { get; set; } = string.Empty;
    public bool ShowRegisterPassword { get; set; } = false;
    public bool ShowConfirmPassword { get; set; } = false;

    public bool AcceptTerms { get; set; } = false;
    public bool IsLoginMode { get; set; } = true;
    public bool IsLoading { get; set; } = false;
    public string? ErrorMessage { get; set; } = null;
    public string? SuccessMessage { get; set; } = null;

    private readonly IJSRuntime _jsRuntime; // Add a private field for IJSRuntime
    private readonly AuthService _authService;
    private readonly CurrentUserService _currentUserService;

    public ConnexionViewModel(AuthService authService, CurrentUserService currentUserService, IJSRuntime jsRuntime) // Inject IJSRuntime
    {
        _authService = authService;
        _currentUserService = currentUserService;
        _jsRuntime = jsRuntime;
    }

    public void ShowLogin() => IsLoginMode = true;
    public void ShowRegister() => IsLoginMode = false;
    public void ToggleLoginPassword() => ShowLoginPassword = !ShowLoginPassword;
    public void ToggleRegisterPassword() => ShowRegisterPassword = !ShowRegisterPassword;
    public void ToggleConfirmPassword() => ShowConfirmPassword = !ShowConfirmPassword;

    public async Task HandleRegister()
    {
        ErrorMessage = null;
        IsLoading = true;

        LoginRequest request = RequestFactory.CreateRegisterRequest(RegisterUsername, RegisterEmail, RegisterPassword, RegisterConfirmPassword);

        SignUpResponse result = await _authService.SignUpAsync(request);

        IsLoading = false;

        if (result != null)
        {
            Console.WriteLine("Token: " + result.Token);
            Console.WriteLine("Utilisateur: " + result.UserDetails.Login);
        }
        else
        {
            ErrorMessage = "Erreur lors de l'inscription. Veuillez réessayer.";
        }
    }

    public async Task HandleLogin()
    {
        ErrorMessage = null;
        IsLoading = true;

        LoginRequest request = RequestFactory.CreateLoginRequest(LoginEmail, LoginPassword);

        LoginResponse result = await _authService.LoginAsync(request);

        IsLoading = false;

        if (result != null)
        {
            Console.WriteLine("Connexion réussie!");
            Console.WriteLine("Token: " + result.Token);
            Console.WriteLine("Utilisateur: " + result.UserDetails.Login);
            // store id globally for all requests now
            _currentUserService.SetUserId(result.UserDetails.UtilisateurId);

            // store the token for later in the storage
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", result.Token);
        }
        else
        {
            ErrorMessage = "Email/Login ou mot de passe incorrect.";
        }
    }

    public void HandleGoogleLogin()
    {
        // Implement Google login logic here
    }
}