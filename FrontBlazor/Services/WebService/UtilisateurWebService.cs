using FrontBlazor.Pages;
using FrontBlazor.Services.GenericService;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Shared;
using Shared.DTO;
using Shared.DTO.Annonce;
using Shared.DTO.Utilisateur;
using System.Net.Http.Json;

namespace FrontBlazor.Services;

public class UtilisateurWebService : ReadableService<UtilisateurViewDTO>, IUtilisateurService
{
    public UtilisateurWebService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<UtilisateurViewDTO?> GetByLoginAsync(string login)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"Utilisateur/login/{login}");
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"❌ GetByLoginAsync failed: {response.StatusCode}");
                return null;
            }

            var result = await response.Content.ReadFromJsonAsync<UtilisateurViewDTO>();
            Console.WriteLine($"✅ User found: {result?.Login}");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Exception in GetByLoginAsync: {ex.Message}");
            return null;
        }
    }

    public async Task<UtilisateurViewDTO> GetUserById(int id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"Utilisateur/{id}");
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"❌ GetByLoginAsync failed: {response.StatusCode}");
            return null;
        }


        var result = await response.Content.ReadFromJsonAsync<UtilisateurViewDTO>();
        Console.WriteLine($"✅ User found: {result?.Login}");
        return result;

        
    }
    public async Task<UtilisateurSettingsDTO> GetUserSettingsById(int id)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"Utilisateur/GetSettings");
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"❌ GetUserSettingsById failed: {response.StatusCode}");
            return null;
        }

        var result = await response.Content.ReadFromJsonAsync<UtilisateurSettingsDTO>();
        Console.WriteLine($"✅ User found: {result?.Login}");
        return result;
    }
    public async Task<APIResponse<object>> PatchUpdateUser(UtilisateurSettingsDTO utilisateurSettingsDTO)
    {
        var body = JsonContent.Create(utilisateurSettingsDTO);
        var response = await PatchWithCredentialsAsync("Utilisateur/PatchSettings", body);

        var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse<object>>();

        if (apiResponse == null)
        {
            return APIResponse<object>.ErrorResponse("Réponse serveur invalide");
        }

        return apiResponse;
    }
    public async Task UpdateNotifMailPreferenceAsync(int userId, bool preference)
    {
        var dto = new UpdateNotifMailDTO
        {
            PreferenceNotifMail = preference
        };

        var request = new HttpRequestMessage(
            HttpMethod.Put,
            $"Utilisateur/notif-mail")
        {
            Content = JsonContent.Create(dto)
        };

        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"❌ UpdateNotifMailPreference failed: {error}");
            throw new Exception(error);
        }
    }
    public async Task<NewsDTO> GetActivity()
    {
        var response = await GetWithCredentialsAsync($"Utilisateur/notificationAndMessagesCount");
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<NewsDTO>();
        return result;
    }

    public async Task<APIResponse<object>> SuppressionCompte(AccountDeletionDTO accountDeletionDTO)
    {
        var body = JsonContent.Create(accountDeletionDTO);

        var response = await DeleteWithCredentialsAsync("Utilisateur/suppressionCompte", body);

        var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse<object>>();

        if (apiResponse == null)
        {
            return APIResponse<object>.ErrorResponse("Réponse serveur invalide");
        }

        return apiResponse;
    }
}