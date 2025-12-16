using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Net.Http.Json;

namespace FrontBlazor.Services;

public class UtilisateurWebService : ReadableService<UtilisateurView>, IUtilisateurService
{
    public UtilisateurWebService(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<UtilisateurView?> GetByLoginAsync(string login)
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

            var result = await response.Content.ReadFromJsonAsync<UtilisateurView>();
            Console.WriteLine($"✅ User found: {result?.Login}");
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Exception in GetByLoginAsync: {ex.Message}");
            return null;
        }
    }
}