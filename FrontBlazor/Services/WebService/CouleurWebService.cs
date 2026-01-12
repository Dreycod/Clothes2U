using System.Net.Http.Json;
using Shared.DTO;
using FrontBlazor.Services.GenericService;
using Shared.DTO.Couleur;
using Shared.DTO.Annonce;

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

        public async Task<EstDeCouleurDTO?> CouleurToEdc(CouleurDTO couleur, AnnonceDTO annonce)
        {
            try
            {
                CreateEstDeCouleurDTO bodyDto = new CreateEstDeCouleurDTO
                {
                    CouleurId = couleur.CouleurId,
                    AnnonceId = annonce.AnnonceId
                };
                var body = JsonContent.Create(bodyDto);
                var response = await PostWithCredentialsAsync("EstDeCouleur", body);
                response.EnsureSuccessStatusCode();
                var edc = await response.Content.ReadFromJsonAsync<EstDeCouleurDTO>();
                return edc;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CouleurToEdc Error: {ex.Message}");
                return null;
            }
        }

        public async Task<CouleurDTO?> AddAsync(CreateCouleurDTO couleur)
        {
            try
            {
                var body = JsonContent.Create(couleur);
                var response = await PostWithCredentialsAsync("Couleur", body);
                response.EnsureSuccessStatusCode();
                var createdCouleur = await response.Content.ReadFromJsonAsync<CouleurDTO>();
                return createdCouleur;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddCouleur Error: {ex.Message}");
                return null;
            }
        }
    }
}
