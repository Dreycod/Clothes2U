using Shared.DTO;
using Shared.DTO.Utilisateur;
using FrontBlazor.Pages;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
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

    public async Task UpdateNotifMailPreferenceAsync(int userId, bool preference)
    {
        var dto = new UpdateNotifMailDTO
        {
            PreferenceNotifMail = preference
        };

        var request = new HttpRequestMessage(
            HttpMethod.Put,
            $"Utilisateur/{userId}/notif-mail")
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

}