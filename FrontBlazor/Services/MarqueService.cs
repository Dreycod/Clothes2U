using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.Services
{
    public class MarqueService : ReadableService<Marque>, IMarqueService<Marque>
    {
        private readonly HttpClient _httpClient;
        public MarqueService(HttpClient httpClient) : base(httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<List<Marque>?> GetAllMarques()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Marque>>("Marque");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAllCategories Error: {ex.Message}");
                return null;
            }
        }

        public Task<Marque?> AddAsync(Marque entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Marque updatedEntity)
        {
            throw new NotImplementedException();
        }
    }
}
