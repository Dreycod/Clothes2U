using Shared.DTO;
using Shared.DTO.LoginRegister;
using FrontBlazor.Services;
using FrontBlazor.Services.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Net;
using FrontBlazor.ViewModel.Generic;
using Microsoft.AspNetCore.Components;

namespace FrontBlazor.ViewModel;

public class LoginViewModel : ClientBaseViewModel

{
    private readonly IAuthService _authService;
    public LoginViewModel(
        IAuthService authService,
        NavigationManager navigationManager,
        INotificationService notificationService
        
        )
        : base(navigationManager, authService, notificationService)
    {
        _authService = authService;
    }

    public async Task<string> HandleRegister(string RegisterUsername, string RegisterEmail, string RegisterPassword, string RegisterConfirmPassword, bool AcceptTerms)
    {
        
        if (!AcceptTerms)
        {
            return "Vous devez accepter les conditions d'utilisation.";
        }

        try
        {
            RegisterRequestDTO registerRequest = new RegisterRequestDTO()
            {
                Login = RegisterUsername,
                Email = RegisterEmail,
                Password = RegisterPassword,
                PasswordConfirm = RegisterConfirmPassword
                
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(registerRequest, null, null);
            bool isValid = Validator.TryValidateObject(registerRequest, validationContext, validationResults, true);
            
            if (!isValid)
            {
                return validationResults.Select(vr => vr.ErrorMessage).First() ?? "Vérifiez votre saisie";
            }
            
            var result = await _authService.SignUpAsync(registerRequest);

            if (result.Success)
            {
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
            LoginRequestDTO requestAuth = new LoginRequestDTO()
            {
                Login = LoginEmail,
                Password = LoginPassword
            };
            var result = await _authService.LoginAsync(requestAuth);

            switch (result)
            {
                case HttpStatusCode.OK:
                    return "Success";

                case HttpStatusCode.Unauthorized:
                    return "Email, Login ou mot de passe incorrect.";

                case HttpStatusCode.BadRequest:
                    return "Requête invalide. Vérifiez vos informations.";

                default:
                    return "Erreur lors de la connexion.";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur: {ex.Message}");
            return $"Erreur réelle: {ex.Message}";
        }
        

        return "";
    }

    public async Task<bool> HandleLogout()
    {
        try
        {
            await _authService.LogoutAsync();
            
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la déconnexion: {ex.Message}");
            return false;
        }
    }

    public async Task OnGoogleClicked()
    {
        var googleUrl = _authService.GetGoogleLoginUrl("/");
        _nav.NavigateTo(googleUrl, forceLoad: true);
    }

    public void HandleGoogleLogin()
    {
        // Implement Google login logic here
    }

    public async Task<bool> CheckLoginStatus()
    {
        if (await _authService.GetCurrentUserAsync() != null)
            return true;
        return false;
    }
}
