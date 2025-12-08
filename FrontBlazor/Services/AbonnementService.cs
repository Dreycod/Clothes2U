using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.Services
{
    public class AbonnementService : ReadableService<Abonnement>, IAbonnementService<Abonnement>
    {
        public AbonnementService(HttpClient httpClient) : base(httpClient) { }
        public Task AddAbonnement(int utilisateurId)
        {
            
            throw new NotImplementedException();
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
