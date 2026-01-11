using Shared.DTO;
using FrontBlazor.Services.GenericService;
using FrontBlazor.Services.Interfaces;
using System.Net.Http.Json;
using Shared.DTO.Abonnement;
using Shared.DTO.Utilisateur;

namespace FrontBlazor.Services
{
    public class AbonnementWebService : BaseGenericService, IAbonnementService
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

        public async Task<List<UtilisateurCardDTO>> GetAbonnements()
        {
            try
            {
                var response = await GetWithCredentialsAsync("Abonnement/abonnements");
                response.EnsureSuccessStatusCode();
                var abonnements = await response.Content.ReadFromJsonAsync<List<UtilisateurCardDTO>>();
                return abonnements;
            }
            catch (Exception ex)
            {
                return new List<UtilisateurCardDTO>();
            }
        }
    }
}
