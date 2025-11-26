using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.Services;

public class AnnonceService : WritableService<Annonce>, IAnnonceService<Annonce>
{
    public AnnonceService(HttpClient httpClient) : base(httpClient) { }

    public async Task<List<Annonce?>?> GetActiveAnnonces()
    {
        return await _httpClient.GetFromJsonAsync<List<Annonce>>(
            "Annonce/GetActiveAnnonces"
        );
    }
    public async Task<List<Annonce?>?> GetAnnoncesByCategorieId(int Id)
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
        return await _httpClient.GetFromJsonAsync<Annonce>(
           $"Annonce/id/{Id}"
       );
    }
    public async Task<List<Annonce?>?> GetAnnoncesByUserIdAsync(int userId)
    {
        return await _httpClient.GetFromJsonAsync<List<Annonce>>(
            $"Annonce/GetAnnoncesByUserId/{userId}"
        );
    }

    public Task<Annonce> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}
