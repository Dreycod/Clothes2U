using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.Services
{
    public class CategorieService : ListableService<Categorie>
    {
        private readonly HttpClient _httpClient;
        public CategorieService(HttpClient httpClient) : base(httpClient)
        {
            _httpClient = httpClient;
        }

        

        public Task<Categorie?> AddAsync(Categorie entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Categorie> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Categorie updatedEntity)
        {
            throw new NotImplementedException();
        }
    }
}
