using System.Net.Http.Json;
using System.Runtime.Serialization;
using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.Services;

public class AnnonceService : WritableService<Annonce>, IAnnonceService<Annonce>
{
    public AnnonceService(HttpClient httpClient) : base(httpClient) {}

    public async Task<List<Annonce>> GetActiveAnnonces()
    {
        var response = await GetWithCredentialsAsync("Annonce/GetActiveAnnonces");
        response.EnsureSuccessStatusCode();

        var annonces = await response.Content.ReadFromJsonAsync<List<Annonce>>();

        return annonces ?? new List<Annonce>();
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

    public Task<List<Annonce>?> GetAnnonceByFilter(FilterDTO filterDto, int page = 1, int pageSize = 3)
    {

        //var response = await GetWithCredentialsAsync($"Annonce/productByFilter?page={page}&pageSize={pageSize}", filterDto);
        //response.EnsureSuccessStatusCode();
        //var annonces = await response.Content.ReadFromJsonAsync<List<Annonce>>();
        //return annonces ?? new List<Annonce>();

        return _httpClient.PostAsJsonAsync($"Annonce/productByFilter?page={page}&pageSize={pageSize}", filterDto)
            .ContinueWith(task =>
            {
                var response = task.Result;
                response.EnsureSuccessStatusCode();
                return response.Content.ReadFromJsonAsync<List<Annonce>>();
            }).Unwrap();
    }
}