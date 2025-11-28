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
            return await _httpClient.GetFromJsonAsync<List<Annonce>>("Annonce/GetActiveAnnonces");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP Error: {ex.Message}");
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
