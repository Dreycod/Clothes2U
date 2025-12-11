using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;
using System.Net.Http.Json;

namespace FrontBlazor.Services
{
    public class UtilisateurWebService : ReadableService<UtilisateurView>, IReadableService<UtilisateurView>
    {
        public UtilisateurWebService(HttpClient httpClient) : base(httpClient) {}

        public async Task<UtilisateurView?> GetByIdAsync(int id)
        {
            var response =  await GetWithCredentialsAsync($"Utilisateur/{id}");
            response.EnsureSuccessStatusCode();
            var utilisateur = await response.Content.ReadFromJsonAsync<UtilisateurView>();
            return utilisateur;
        }
    }
}
