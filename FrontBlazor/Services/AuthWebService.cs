using System.Net;
using System.Net.Http.Json;
using Shared.DTO;
using Shared.DTO.Utilisateur;
using FrontBlazor.Services.GenericIServices;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Shared.DTO.LoginRegister;

namespace FrontBlazor.Services;

public class AuthWebService : BaseGenericService, IAuthService
{
    public AuthWebService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<UtilisateurViewDTO?> GetCurrentUserAsync()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "Login/me");
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<UtilisateurViewDTO>();
        }
        catch
        {
            return null;
        }
    }

    public async Task<AuthResult> SignUpAsync(RegisterRequestDTO compte)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "Login/signup")
            {
                Content = JsonContent.Create(compte)
            };
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                return new AuthResult { Success = false, ErrorMessage = error };
            }

            var result = await response.Content.ReadFromJsonAsync<LoginResponse>();

            return new AuthResult
            {
                Success = true,
                Utilisateur = result?.utilisateur
            };
        }
        catch (Exception ex)
        {
            return new AuthResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    public async Task<HttpStatusCode> LoginAsync(LoginRequestDTO compte)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "Login")
            {
                Content = JsonContent.Create(compte)
            };

            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
            var response = await _httpClient.SendAsync(request);
            
            // Ne pas lancer d'exception si 401 (credentials invalides)
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return HttpStatusCode.Unauthorized;
            }
            
            if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                return HttpStatusCode.BadRequest;
            }
            
            response.EnsureSuccessStatusCode();
            return response.StatusCode;
        }
        catch
        {
            return HttpStatusCode.InternalServerError;
        }
    }

    public async Task LogoutAsync()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "Login/logout");
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

            await _httpClient.SendAsync(request);
        }
        catch { }
    }
    public string GetGoogleLoginUrl(string returnUrl = "/")
    {
        var baseUrl = _httpClient.BaseAddress?.ToString().TrimEnd('/');
        return $"{baseUrl}/Login/google-login?returnUrl={Uri.EscapeDataString(returnUrl)}";
    }
}

public class LoginResponse
{
    public UtilisateurDTO utilisateur { get; set; } = new();
}

public class AuthResult
{
    public bool Success { get; set; }
    public UtilisateurDTO? Utilisateur { get; set; }
    public string? ErrorMessage { get; set; }
}
