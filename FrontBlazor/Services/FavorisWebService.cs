using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;
using System.Net.Http.Json;

namespace FrontBlazor.Services
{
    public class FavorisWebService : WritableService<Favoris>, IFavorisService<Favoris>
    {
        public FavorisWebService(HttpClient httpClient) : base(httpClient) { }

        public Task<Favoris> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task AddFavoris(int annonceId)
        {
            var body = JsonContent.Create(annonceId);

            var response = await PostWithCredentialsAsync("Favoris", body);
        }
        public async Task DeleteFavoris(int annonceId)
        {
            await DeleteWithCredentialsAsync($"Favoris/id/{annonceId}");
        }
    }
}
