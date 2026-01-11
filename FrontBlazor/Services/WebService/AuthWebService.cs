using FrontBlazor.Services.GenericService;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Shared;
using Shared.DTO;
using Shared.DTO.LoginRegister;
using Shared.DTO.Utilisateur;
using Stripe;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Http.Json;

namespace FrontBlazor.Services;

public class AuthWebService : BaseGenericService, IAuthService
{
    public AuthWebService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<CurrentUtilisateurDTO?> GetCurrentUserAsync()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "Login/me");
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<CurrentUtilisateurDTO>();
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

    public async Task<APIResponse<object>> ModificationMotDePasse(ChangePasswordDTO passwordDTO)
    {
        var body = JsonContent.Create(passwordDTO);

        var response = await PatchWithCredentialsAsync("Login/modificationMotDePasse", body);

        var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse<object>>();

        if (apiResponse == null)
        {
            return APIResponse<object>.ErrorResponse("Réponse serveur invalide");
        }

        return apiResponse;
    }

    public async Task<List<AdresseDTO>> GetUserAddressesAsync()
    {
        try
        {
            var request = await GetWithCredentialsAsync("Address/user");
            return await request.Content.ReadFromJsonAsync<List<AdresseDTO>>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error getting addresses: {ex.Message}");
            return new List<AdresseDTO>();
        }
    }

    public async Task<AdresseDTO> AddAddressAsync(CreateAdresseDTO address)
    {
        var response = await PostWithCredentialsAsync("address", JsonContent.Create(address));
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<AdresseDTO>();
        return result ?? throw new Exception("Failed to add address");
    }

    public async Task<bool> UpdateAddressAsync(int addressId, UpdateAdresseDTO address)
    {
        var response = await PutWithCredentialsAsync($"address/{addressId}", JsonContent.Create(address));
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAddressAsync(int addressId)
    {
        var response = await DeleteWithCredentialsAsync($"address/{addressId}");
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> SetDefaultAddressAsync(int addressId)
    {
        var response = await PutWithCredentialsAsync($"address/{addressId}/set-default", null);
        return response.IsSuccessStatusCode;
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
