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
        return await _httpClient.GetFromJsonAsync<List<AnnonceDetail>>(
            $"Annonce/ByCategorieId/{Id}"
        );
    }
    public async Task<List<AnnonceDetail?>?> GetAnnoncesBySousCategoryId(int Id)
    {
        return await _httpClient.GetFromJsonAsync<List<AnnonceDetail>>(
             $"Annonce/BySousCategorieId/{Id}"
         );
    }
    public async Task<AnnonceDetail> GetAnnonceDetailById(int Id)
    {
        return await _httpClient.GetFromJsonAsync<AnnonceDetail>(
           $"Annonce/id/{Id}"
       );
    }
    public async Task<List<Annonce?>?> GetAnnoncesByUserIdAsync(int userId)
    {
        return await _httpClient.GetFromJsonAsync<List<Annonce>>(
            $"Annonce/GetAnnoncesByUserId/{userId}"
        );
    }

    // le problème c'est que annonceService doit utiliser des annonce detail, et pas annonce simple pour certaines méthodes
    // mais on doit faire pour les deux, donc rip

    Task<List<Annonce>?> IAnnonceService<Annonce>.GetAnnoncesByCategorieId(int id)
    {
        throw new NotImplementedException();
    }

    Task<List<Annonce>?> IAnnonceService<Annonce>.GetAnnoncesBySousCategoryId(int id)
    {
        throw new NotImplementedException();
    }

    Task<List<Annonce>?> IAnnonceService<Annonce>.GetAnnonceDetailById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Annonce> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
}
