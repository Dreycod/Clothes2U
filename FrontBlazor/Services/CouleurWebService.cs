using System.Net.Http.Json;
using Shared.DTO;
using FrontBlazor.Services.GenericService;
using Shared.DTO.Couleur;

namespace FrontBlazor.Services
{
    public class CouleurWebService : BaseGenericService, ICouleurService<CouleurDTO>
    {
        private readonly HttpClient _httpClient;
        public CouleurWebService(HttpClient httpClient) : base(httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<List<CouleurDTO>?> GetAllAsync()
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

        public async Task<CouleurDTO?> GetByIdAsync(int id)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<CouleurDTO>($"Couleur/{id}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetCouleurById Error: {ex.Message}");
                return null;
            }
        }

        public Task<CouleurDTO?> AddAsync(CouleurDTO entity)
        {
            try
            {
                var body = JsonContent.Create(entity);
                var response = PostWithCredentialsAsync("Couleur", body);
                response.Result.EnsureSuccessStatusCode();
                var createdCouleur = response.Result.Content.ReadFromJsonAsync<CouleurDTO>();
                return createdCouleur;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddCouleur Error: {ex.Message}");
                return Task.FromResult<CouleurDTO?>(null);
            }
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
