using System.Net.Http.Json;
using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.Services
{
    public class CouleurWebService : ReadableService<Couleur>, ICouleurService<Couleur>
    {
        private readonly HttpClient _httpClient;
        public CouleurWebService(HttpClient httpClient) : base(httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<List<Couleur>?> GetAllCouleurs()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<Couleur>>("Couleur");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAllCouleur Error: {ex.Message}");
                return null;
            }
        }

        public Task<Couleur?> AddAsync(Couleur entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(Couleur updatedEntity)
        {
            throw new NotImplementedException();
        }
    }
}
