using System.Net.Http.Json;
using Shared.DTO;
using FrontBlazor.Services.GenericIServices;
using Shared.DTO.Couleur;

namespace FrontBlazor.Services
{
    public class CouleurWebService : ReadableService<CouleurDTO>, ICouleurService<CouleurDTO>
    {
        private readonly HttpClient _httpClient;
        public CouleurWebService(HttpClient httpClient) : base(httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<List<CouleurDTO>?> GetAllCouleurs()
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<List<CouleurDTO>>("Couleur");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAllCouleur Error: {ex.Message}");
                return null;
            }
        }

        public Task<CouleurDTO?> AddAsync(CouleurDTO entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(CouleurDTO updatedEntity)
        {
            throw new NotImplementedException();
        }
    }
}
