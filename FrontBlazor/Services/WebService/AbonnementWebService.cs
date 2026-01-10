using Shared.DTO;
using FrontBlazor.Services.GenericService;
using FrontBlazor.Services.Interfaces;
using System.Net.Http.Json;
using Shared.DTO.Abonnement;

namespace FrontBlazor.Services
{
    public class AbonnementWebService : ReadableService<AbonnementDTO>, IAbonnementService<AbonnementDTO>
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

        public Task<AbonnementDTO?> AddAsync(AbonnementDTO entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(AbonnementDTO updatedEntity)
        {
            throw new NotImplementedException();
        }
    }
}
