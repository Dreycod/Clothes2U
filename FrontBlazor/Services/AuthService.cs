using System.Net.Http.Json;
using FrontBlazor.Models;
using FrontBlazor.Models.LoginRegister;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.Services;

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthResult> LoginAsync(string loginOrEmail, string password)
    {
        try
        {
            var request = new LoginRequest
            {
                Login = loginOrEmail,
                Email = loginOrEmail,
                Password = password
            };

            var response = await _httpClient.PostAsJsonAsync("Login", request);

            if (response.IsSuccessStatusCode)
            {
                var utilisateur = await response.Content.ReadFromJsonAsync<Utilisateur>();
                if (utilisateur == null)
                    return new AuthResult
                    {
                        Success = false,
                        ErrorMessage = "Utilisateur reçu est null"
                    };

                return new AuthResult
                {
                    Success = true,
                    Utilisateur = utilisateur
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = errorContent.Trim('"')
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception dans LoginAsync: {ex.Message}");
            return new AuthResult
            {
                Success = false,
                ErrorMessage = $"Erreur de connexion: {ex.Message}"
            };
        }
    }

    public async Task<AuthResult> SignUpAsync(string email, string login, string password, string passwordConfirm)
    {
        try
        {
            var request = new LoginRequest
            {
                Email = email,
                Login = login,
                Password = password,
                PasswordConfirm = passwordConfirm
            };

            var response = await _httpClient.PostAsJsonAsync("Login/signup", request);

            if (response.IsSuccessStatusCode)
            {
                var utilisateur = await response.Content.ReadFromJsonAsync<Utilisateur>();
                return new AuthResult
                {
                    Success = true,
                    Utilisateur = utilisateur
                };
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return new AuthResult
                {
                    Success = false,
                    ErrorMessage = errorContent.Trim('"')
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception dans SignUpAsync: {ex.Message}");
            return new AuthResult
            {
                Success = false,
                ErrorMessage = $"Erreur d'inscription: {ex.Message}"
            };
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            await _httpClient.PostAsync("Login/logout", null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la déconnexion: {ex.Message}");
        }
    }

    public async Task<Utilisateur?> GetCurrentUserAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("Login/me");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Utilisateur>();
            }

            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur GetCurrentUserAsync: {ex.Message}");
            return null;
        }
    }
}

public class AuthResult
{
    public bool Success { get; set; }
    public Utilisateur? Utilisateur { get; set; }
    public string? ErrorMessage { get; set; }
}
