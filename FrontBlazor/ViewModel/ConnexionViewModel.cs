using FrontBlazor.Models;
using FrontBlazor.Services;
using Microsoft.JSInterop;

namespace FrontBlazor.ViewModel;

public class ConnexionViewModel
{
    private readonly IJSRuntime _jsRuntime; // Add a private field for IJSRuntime
    private readonly AuthService _authService;
    private readonly CurrentUserService _currentUserService;

    public ConnexionViewModel(AuthService authService, CurrentUserService currentUserService, IJSRuntime jsRuntime) // Inject IJSRuntime
    {
        _authService = authService;
        _currentUserService = currentUserService;
        _jsRuntime = jsRuntime;
    }

    public async Task<string> HandleRegister(string RegisterUsername, string RegisterEmail, string RegisterPassword, string RegisterConfirmPassword, bool AcceptTerms)
    {
        LoginRequest request = RequestFactory.CreateRegisterRequest(RegisterUsername, RegisterEmail, RegisterPassword, RegisterConfirmPassword);

        // gotta get the badrequests' text when it fails, and not exactly null
        SignUpResponse result = await _authService.SignUpAsync(request);

        if (result != null)
        {
            Console.WriteLine("Token: " + result.Token);
            Console.WriteLine("Utilisateur: " + result.UserDetails.Login);

            // go to home page
            return "Success" ;
        }
        else
        {
            return "Erreur lors de l'inscription. Veuillez réessayer.";
        }
    }

    public async Task<string> HandleLogin(string LoginEmail, string LoginPassword)
    {
        LoginRequest request = RequestFactory.CreateLoginRequest(LoginEmail, LoginPassword);

        LoginResponse result = await _authService.LoginAsync(request);

        if (result != null)
        {
            Console.WriteLine("Connexion réussie!");
            Console.WriteLine("Token: " + result.Token);
            Console.WriteLine("Utilisateur: " + result.UserDetails.Login);
            // store id globally for all requests now
            _currentUserService.SetUserId(result.UserDetails.UtilisateurId);

            // store the token for later in the storage
            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", result.Token);

            // go to home page
            return "Success";
        }
        else
        {

            return "Email/Login ou mot de passe incorrect.";
        }
    }

    public void HandleGoogleLogin()
    {
        // Implement Google login logic here
    }
}