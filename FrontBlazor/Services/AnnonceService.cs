using FrontBlazor.Models;

namespace FrontBlazor.Services
{
    public class AnnonceService : ListableService<AnnonceDTO>
    {
        public AnnonceService(HttpClient httpClient) : base(httpClient) { }

        public async Task<List<AnnonceDTO>?> GetActiveAnnoncesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<AnnonceDTO>>(
                "Annonce/GetActiveAnnonces"
            );
        }
        public async Task<List<AnnonceDTO>?> GetAnnoncesByUserIdAsync(int userId)
        {
            return await _httpClient.GetFromJsonAsync<List<AnnonceDTO>>(
                $"Annonce/GetAnnoncesByUserId/{userId}"
            );
        }
    }
}
