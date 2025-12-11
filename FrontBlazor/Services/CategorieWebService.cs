using System.Net.Http.Json;
using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.Services
{
    public class CategorieWebService : ReadableService<Categorie>, ICategorieService<Categorie>
    {
        private readonly HttpClient _httpClient;
        public CategorieWebService(HttpClient httpClient) : base(httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Categorie>?> GetAllCategories()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Categorie>>("Categorie");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAllCategories Error: {ex.Message}");
                return null;
            }
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
