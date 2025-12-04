using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;
using System.Net.Http.Json;

namespace FrontBlazor.Services
{
    public class FavorisService : WritableService<Favoris>, IFavorisService<Favoris>
    {
        public FavorisService(HttpClient httpClient) : base(httpClient) { }

        public Task<Favoris> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task AddFavoris(int annonceId)
        {
            var body = JsonContent.Create(new { AnnonceId = annonceId });

            var response = await PostWithCredentialsAsync("Favoris", body);
            response.EnsureSuccessStatusCode();
        }
        public async Task DeleteFavoris(int annonceId)
        {
            await DeleteWithCredentialsAsync($"Favoris/id/{annonceId}");
        }
    }
}
