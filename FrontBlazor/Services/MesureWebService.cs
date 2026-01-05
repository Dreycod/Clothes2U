using FrontBlazor.Services.GenericIServices;
using FrontBlazor.Services.Interfaces;
using Shared.DTO.Mesures;
using System.Net.Http.Json;

namespace FrontBlazor.Services;

public class MesureWebService : BaseGenericService, IMesureService
{
    public MesureWebService(HttpClient httpClient) : base(httpClient) { }

    public async Task<List<MesureDTO>?> GetAllMesuresAsync()
    {
        try
        {
            var response = await GetWithCredentialsAsync("Mesure");
            response.EnsureSuccessStatusCode();

            var mesures = await response.Content.ReadFromJsonAsync<List<MesureDTO>>();
            return mesures ?? new List<MesureDTO>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur lors de la récupération des mesures: {ex.Message}");
            return null;
        }
    }

    public async Task<List<int>?> GetTailleIdsByCategorieAsync(int categorieId)
    {
        try
        {
            var response = await GetWithCredentialsAsync($"Mesure/byCategorie/{categorieId}");
            response.EnsureSuccessStatusCode();

            var tailleIds = await response.Content.ReadFromJsonAsync<List<int>>();
            return tailleIds ?? new List<int>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Erreur lors de la récupération des tailles pour catégorie {categorieId}: {ex.Message}");
            return null;
        }
    }
}