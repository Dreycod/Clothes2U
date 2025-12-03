using System.Net;
using System.Net.Http.Json;
using FrontBlazor.Models;
using FrontBlazor.Models.LoginRegister;
using FrontBlazor.Services.GenericIServices;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace FrontBlazor.Services;

public class AuthService : BaseGenericService, IAuthService
{
    public AuthService(HttpClient httpClient) : base(httpClient)
    {
        
    }

    public async Task<Utilisateur?> GetCurrentUserAsync()
    {
        try
        {
            var response = await _httpClient.GetAsync("Login/me");

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<Utilisateur>();
        }
        catch
        {
            return null;
        }
    }

    public async Task<AuthResult> SignUpAsync(LoginRequest compte)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("Login/signup", compte);

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

    public async Task<HttpStatusCode> LoginAsync(LoginRequest compte)
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
            await _httpClient.PostAsync("Login/logout", null);
        }
        catch { }
    }
}

public class LoginResponse
{
    public Utilisateur utilisateur { get; set; } = new();
}

public class AuthResult
{
    public bool Success { get; set; }
    public Utilisateur? Utilisateur { get; set; }
    public string? ErrorMessage { get; set; }
}
