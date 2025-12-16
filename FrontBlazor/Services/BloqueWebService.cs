using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;
using System.Net.Http.Json;

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

        public Task<Bloque?> AddAsync(Bloque entity)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteAsync(int id)
        {
            await DeleteWithCredentialsAsync($"Bloque/{id}");
        }

        public Task UpdateAsync(Bloque updatedEntity)
        {
            throw new NotImplementedException();
        }
    }
}
