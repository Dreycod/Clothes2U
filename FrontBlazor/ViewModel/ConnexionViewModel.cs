using FrontBlazor.Models;
using FrontBlazor.Models.LoginRegister;
using FrontBlazor.Models.StateServices;
using FrontBlazor.Services;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.ViewModel;

public class ConnexionViewModel
{
    private readonly IAuthService _authService;
    private readonly IStateService<Utilisateur> _userStateService;

    public ConnexionViewModel(IAuthService authService, IStateService<Utilisateur> userStateService)
    {
        _authService = authService;
        _userStateService = userStateService;
    }

    public async Task<string> HandleRegister(string RegisterUsername, string RegisterEmail, string RegisterPassword, string RegisterConfirmPassword, bool AcceptTerms)
    {
        if (!AcceptTerms)
        {
            return "Vous devez accepter les conditions d'utilisation.";
        }

        try
        {
            var result = await _authService.SignUpAsync(RegisterEmail, RegisterUsername, RegisterPassword, RegisterConfirmPassword);

            if (result.Success)
            {
                Console.WriteLine("Inscription réussie!");

                _userStateService.CurrentEntity = result.Utilisateur;

                Console.WriteLine("L'utilisateur est : " + _userStateService.CurrentEntity.Login);
                return "Success";
            }
            else
            {
                return result.ErrorMessage ?? "Erreur lors de l'inscription. Veuillez réessayer.";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur: {ex.Message}");
            return "Une erreur est survenue lors de l'inscription.";
        }
    }

    public async Task<string> HandleLogin(string LoginEmail, string LoginPassword)
    {
        if (string.IsNullOrWhiteSpace(LoginEmail) || string.IsNullOrWhiteSpace(LoginPassword))
        {
            return "Veuillez remplir tous les champs.";
        }

        try
        {
            var result = await _authService.LoginAsync(LoginEmail, LoginPassword);

            if (result.Success)
            {
                Console.WriteLine("Connexion réussie!");
                Console.WriteLine("Utilisateur: " + result.Utilisateur.Login);

                _userStateService.CurrentEntity = result.Utilisateur;

                return "Success";
            }
            else
            {
                return result.ErrorMessage ?? "Email/Login ou mot de passe incorrect.";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur: {ex.Message}");
            return $"Erreur réelle: {ex.Message}";
        }
    }

    public async Task<bool> HandleLogout()
    {
        try
        {
            await _authService.LogoutAsync();
            
            _userStateService.CurrentEntity = null;
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la déconnexion: {ex.Message}");
            return false;
        }
    }

    public void HandleGoogleLogin()
    {
        // Implement Google login logic here
    }
}
