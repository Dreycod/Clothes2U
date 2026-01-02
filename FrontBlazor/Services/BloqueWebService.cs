using Shared.DTO;
using FrontBlazor.Services.GenericIServices;
using System.Net.Http.Json;
using Shared.DTO.Bloque;

namespace FrontBlazor.Services
{
    public class BloqueWebService : BaseGenericService, IBloqueService
    {
        public BloqueWebService(HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task CreateBloque(int utilisateurBloqueId)
        {
            var body = JsonContent.Create(utilisateurBloqueId);

            var response = await PostWithCredentialsAsync("Bloque", body);
        }

        public Task<BloqueDTO?> AddAsync(BloqueDTO entity)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(int id)
        {
            await DeleteWithCredentialsAsync($"Bloque/{id}");
        }

        public Task UpdateAsync(BloqueDTO updatedEntity)
        {
            throw new NotImplementedException();
        }
        public async Task<List<BloqueDetailDTO>?> GetUsersBloquee(int? id)
        {
            var response = await GetWithCredentialsAsync($"Bloque/bloqueur/{id}");
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<List<BloqueDetailDTO>>();
            return result ?? null;
        }
    }
}
