using FrontBlazor.Models.Annonces;
using FrontBlazor.Services.GenericIServices;

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
        public async Task<List<AnnonceDTO>?> GetAnnoncesByFiltersAsync(AnnonceSearchRequestDTO filterRequest)
        {
            var response = await _httpClient.PostAsJsonAsync("Annonce/Search", filterRequest);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("SignUp Error: " + error);
                return null;
            }
            return await response.Content.ReadFromJsonAsync<List<AnnonceDTO>>();
        }
    }
}
