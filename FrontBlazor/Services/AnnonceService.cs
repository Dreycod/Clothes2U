using FrontBlazor.Models.Annonces;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.Services;

public class AnnonceService : WritableService<Annonce>
{
    public AnnonceService(HttpClient httpClient) : base(httpClient) { }

    public async Task<List<Annonce>?> GetActiveAnnoncesAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<Annonce>>(
            "Annonce/GetActiveAnnonces"
        );
    }
    public async Task<List<Annonce>?> GetAnnoncesByUserIdAsync(int userId)
    {
        return await _httpClient.GetFromJsonAsync<List<Annonce>>(
            $"Annonce/GetAnnoncesByUserId/{userId}"
        );
    }

    public async Task<List<AnnonceDetail>?> GetAnnonceDetailById(int Id)
    {
        return await _httpClient.GetFromJsonAsync<List<AnnonceDetail>>(
           $"Annonce/id/{Id}"
       );
    }
}
