using FrontBlazor.Models;
using FrontBlazor.Services;

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


    public void ShowLogin() => IsLoginMode = true;
    public void ShowRegister() => IsLoginMode = false;
    public void ToggleLoginPassword() => ShowLoginPassword = !ShowLoginPassword;
    public void ToggleRegisterPassword() => ShowRegisterPassword = !ShowRegisterPassword;
    public void ToggleConfirmPassword() => ShowConfirmPassword = !ShowConfirmPassword;
    
   // private readonly WritableService<LoginRequest> _utilisateurService;
    private readonly AuthService _authService;

    public ConnexionViewModel(AuthService authService)
    {
        //_utilisateurService = utilisateurService;
        _authService = authService;
    }

    public async Task HandleRegister()
    {
        ErrorMessage = null;
        IsLoading = true;

        var request = new LoginRequest
        {
            Login = RegisterUsername,
            Email = RegisterEmail,
            Password = RegisterPassword,
            PasswordConfirm = RegisterConfirmPassword
        };

        var result = await _authService.SignUpAsync(request);

        IsLoading = false;

        if (result != null)
        {
            // Success! Store token and redirect
            Console.WriteLine("Inscription réussie: " + result.Message);
            Console.WriteLine("Token: " + result.Token);
            Console.WriteLine("Utilisateur: " + result.UserDetails.Login);

            // TODO: Store token (localStorage, session, etc.)
            // TODO: Navigate to home page
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

        var request = new LoginRequest
        {
            Login = LoginEmail,
            Password = LoginPassword
        };

        var result = await _authService.LoginAsync(request);

        IsLoading = false;

        if (result != null)
        {
            // Success! Store token and redirect
            Console.WriteLine("Connexion réussie!");
            Console.WriteLine("Token: " + result.Token);
            Console.WriteLine("Utilisateur: " + result.UserDetails.Login);

            // TODO: Store token
            // TODO: Navigate to home page
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