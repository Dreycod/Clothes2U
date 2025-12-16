using System.Net.Http.Json;
using System.Text.Json;
using FrontBlazor.Models.Moderation;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Http;

namespace FrontBlazor.Services;

public class MotsInterditWebService : BaseGenericService, IMotsInterditsService
{
    public MotsInterditWebService(HttpClient httpClient) : base(httpClient){}
    public async Task<List<MotInterdit>> GetAllAsync()
    {
        try
        {
            var response = await GetWithCredentialsAsync("MotInterdit");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Erreur {response.StatusCode}: {errorContent}");
                return new List<MotInterdit>();
            }
            
            var result = await response.Content.ReadFromJsonAsync<List<MotInterdit>>();
            return result ?? new List<MotInterdit>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
            return new List<MotInterdit>();
        }
    }

    public async Task DeleteAsync(int id)
    {
        try
        {
            var response = await DeleteWithCredentialsAsync($"MotInterdit/id/{id}");
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception: {ex.Message}");
        }
    }

    public async Task<(MotInterdit? mot, string? error)> AddAsync(MotInterdit entity)
    {
        try
        {
            var body = JsonContent.Create(entity);
            var response = await PostWithCredentialsAsync("MotInterdit", body);
        
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                try
                {
                    var options = new JsonSerializerOptions 
                    { 
                        PropertyNameCaseInsensitive = true 
                    };
                    var errorObj = JsonSerializer.Deserialize<Dictionary<string, string>>(errorContent, options);
                    if (errorObj != null && errorObj.ContainsKey("LibelleMot"))
                    {
                        return (null, errorObj["LibelleMot"]);
                    }
                    else if (errorObj != null && errorObj.ContainsKey("libelleMot"))
                    {
                        return (null, errorObj["libelleMot"]);
                    }
                    else if (errorObj != null && errorObj.ContainsKey("message"))
                    {
                        return (null, errorObj["message"]);
                    }
                }
                catch (JsonException jsonEx)
                {
                    Console.WriteLine($"Erreur de désérialisation: {jsonEx.Message}");
                    Console.WriteLine($"Contenu reçu: {errorContent}");
                }
            
                return (null, errorContent);
            }
        
            var result = await response.Content.ReadFromJsonAsync<MotInterdit>();
            return (result, null);
        }
        catch (Exception ex)
        {
            return (null, $"Erreur lors de l'ajout: {ex.Message}");
        }
    }
}