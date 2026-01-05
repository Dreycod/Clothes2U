using System.Net.Http.Json;
using Shared.DTO.Historique;

namespace FrontBlazor.Services
{
    public class TransactionService : BaseGenericService
    {
        public TransactionService(HttpClient httpClient) : base(httpClient) { }

        public async Task<List<TransactionHistoriqueDTO>> GetHistoriqueAcheteurAsync(int userId)
        {

            var response = await GetWithCredentialsAsync($"transaction/historique/acheteur/{userId}");
            if (!response.IsSuccessStatusCode) return new();
            var json = await response.Content.ReadAsStringAsync();
            await Console.Out.WriteLineAsync(json);

            return await response.Content.ReadFromJsonAsync<List<TransactionHistoriqueDTO>>() ?? new();
        }

        public async Task<List<TransactionHistoriqueDTO>> GetHistoriqueVendeurAsync(int userId)
        {
            var response = await GetWithCredentialsAsync($"transaction/historique/vendeur/{userId}");
            if (!response.IsSuccessStatusCode) return new();

            return await response.Content.ReadFromJsonAsync<List<TransactionHistoriqueDTO>>() ?? new();
        }


    }
}
