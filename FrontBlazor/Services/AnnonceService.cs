using System.Net.Http.Json;
using System.Runtime.Serialization;
using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.Services;

public class AnnonceService : WritableService<Annonce>, IAnnonceService<Annonce>
{
    public AnnonceService(HttpClient httpClient) : base(httpClient) {}
    public async Task<List<Annonce>?> GetActiveAnnonces()
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "Annonce/GetActiveAnnonces");

            // Ajouter un en-tête personnalisé si nécessaire
            request.Headers.Add("Authorization", "Bearer YOUR_TOKEN");

            // Log des headers
            Console.WriteLine("Request Headers:");
            foreach (var header in request.Headers)
            {
                Console.WriteLine($"{header.Key}: {string.Join(", ", header.Value)}");
            }

            // Envoi de la requête
            var response = await _httpClient.SendAsync(request);

            // Vérifier la réponse
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Annonce>>() ?? new List<Annonce>();
            }

            // Gestion des erreurs HTTP
            Console.WriteLine($"Error: {response.StatusCode}");
            return null;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP Error (GetActiveAnnonces): {ex.Message}");
            return null;
        }
    }

    public async Task<List<Annonce>?> GetAnnoncesByCategorieId(int Id)
    {
        return await _httpClient.GetFromJsonAsync<List<Annonce>>(
            $"Annonce/ByCategorieId/{Id}"
        );
    }
    public async Task<List<Annonce?>?> GetAnnoncesBySousCategoryId(int Id)
    {
        return await _httpClient.GetFromJsonAsync<List<Annonce>>(
             $"Annonce/BySousCategorieId/{Id}"
         );
    }
    public async Task<Annonce> GetAnnonceDetailById(int Id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Annonce>($"Annonce/id/{Id}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP Error: {ex.Message}");
            return null;
        }
    }
    public async Task<List<Annonce?>?> GetAnnoncesByUserIdAsync(int userId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<Annonce>>($"Annonce/GetAnnoncesByUserId/{userId}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP Error: {ex.Message}");
            return null;
        }
    }

    public Task<Annonce> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

}
