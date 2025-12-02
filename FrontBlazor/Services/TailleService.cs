using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;
using System.Net.Http;
using System.Net.Http.Json;

namespace FrontBlazor.Services
{
    public class TailleService : ReadableService<Taille>, ITailleService<Taille>
    {
        private readonly HttpClient _httpClient;
        public TailleService(HttpClient httpClient) : base(httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Taille>?> GetAllTailles()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Taille>>("Taille");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAllCategories Error: {ex.Message}");
                return null;
            }
        }

        public Task<Taille?> AddAsync(Taille entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Taille> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Taille updatedEntity)
        {
            throw new NotImplementedException();
        }
    }
}
