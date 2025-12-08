using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;
using System.Net.Http.Json;

namespace FrontBlazor.Services
{
    public class UtilisateurService : ReadableService<UtilisateurView>, IReadableService<UtilisateurView>
    {
        public UtilisateurService(HttpClient httpClient) : base(httpClient) {}

        public async Task<UtilisateurView?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<UtilisateurView>($"Utilisateur/{id}");
        }
    }
}
