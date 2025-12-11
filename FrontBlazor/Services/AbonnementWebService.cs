using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;
using System.Net.Http.Json;

namespace FrontBlazor.Services
{
    public class AbonnementWebService : ReadableService<Abonnement>, IAbonnementService<Abonnement>
    {
        public AbonnementWebService(HttpClient httpClient) : base(httpClient) { }
        public async Task AddAbonnement(int utilisateurId)
        {
            var body = JsonContent.Create(utilisateurId);

            var response = await PostWithCredentialsAsync("Abonnement", body);
        }
        public async Task DeleteAbonnement(int utilisateurId)
        {
            await DeleteWithCredentialsAsync($"Abonnement/{utilisateurId}");
        }

        public Task<Abonnement?> AddAsync(Abonnement entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Abonnement updatedEntity)
        {
            throw new NotImplementedException();
        }
    }
}
