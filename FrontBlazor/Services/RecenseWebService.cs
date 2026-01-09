using FrontBlazor.Services.Interfaces;
using FrontBlazor.Services.GenericService;
using Shared.DTO.Annonce;
using Shared.DTO.Recense;
using System.Net.Http.Json;

namespace FrontBlazor.Services
{
    public class RecenseWebService : BaseGenericService, IRecenseService<RecenseDetailDTO>
    {
        public RecenseWebService(HttpClient httpClient) : base(httpClient) {}

        public Task<RecenseDetailDTO> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<RecenseDetailDTO>> GetTagsByAnnonce(int id)
        {
            var response = await GetWithCredentialsAsync($"Recense/TagsByAnnonce/{id}");
            response.EnsureSuccessStatusCode();

            var tags = await response.Content.ReadFromJsonAsync<List<RecenseDetailDTO>>();

            return tags ?? new List<RecenseDetailDTO>();
        }
    }
}
