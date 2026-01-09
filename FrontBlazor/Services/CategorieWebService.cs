using System.Net.Http.Json;
using Shared.DTO;
using Shared.DTO.Categorie;
using FrontBlazor.Services.GenericService;
using FrontBlazor.Services.Interfaces;

namespace FrontBlazor.Services
{
    public class CategorieWebService : ReadableService<CategorieDTO>, ICategorieService<CategorieDTO>
    {
        private readonly HttpClient _httpClient;
        public CategorieWebService(HttpClient httpClient) : base(httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CategorieDTO>?> GetAllCategories()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<CategorieDTO>>("Categorie");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAllCategories Error: {ex.Message}");
                return null;
            }
        }

        public Task<CategorieDTO?> AddAsync(CategorieDTO entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<CategorieDTO> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(CategorieDTO updatedEntity)
        {
            throw new NotImplementedException();
        }
    }
}
