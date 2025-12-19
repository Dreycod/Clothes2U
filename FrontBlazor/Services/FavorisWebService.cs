using Shared.DTO;
using FrontBlazor.Services.GenericIServices;
using System.Net.Http.Json;
using Shared.DTO.Favoris;

namespace FrontBlazor.Services
{
    public class FavorisWebService : WritableService<FavorisDTO>, IFavorisService<FavorisDTO>
    {
        public FavorisWebService(HttpClient httpClient) : base(httpClient) { }

        public Task<FavorisDTO> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public async Task AddFavoris(int annonceId)
        {
            var body = JsonContent.Create(annonceId);
            await PostWithCredentialsAsync("Favoris", body);
        }
        public async Task DeleteFavoris(int annonceId)
        {
            await DeleteWithCredentialsAsync($"Favoris/id/{annonceId}");
        }
    }
}
