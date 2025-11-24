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
    public async Task<List<AnnonceDetail?>?> GetAnnoncesByCategorieId(int Id)
    {
        return new List<AnnonceDetail?>();
    }
    public async Task<List<AnnonceDetail?>?> GetAnnoncesBySousCategoryId(int Id)
    {
       throw new NotImplementedException();
    }
    public async Task<List<AnnonceDetail?>?> GetAnnonceDetailById(int Id)
    {
        return await _httpClient.GetFromJsonAsync<List<AnnonceDetail>>(
           $"Annonce/id/{Id}"
       );
    }
    public async Task<List<Annonce?>?> GetAnnoncesByUserIdAsync(int userId)
    {
        return await _httpClient.GetFromJsonAsync<List<Annonce>>(
            $"Annonce/GetAnnoncesByUserId/{userId}"
        );
    }
}
