using System.Net.Http.Json;
using System.Text.Json;
using FrontBlazor.Services.Interfaces;
using Shared;
using Shared.DTO.DemandeRestauration;

namespace FrontBlazor.Services;

public class DemandeRestaurationWebService : BaseGenericService,  IDemandeRestaurationService
{
    private readonly HttpClient _httpClient;
    public DemandeRestaurationWebService(HttpClient httpClient) : base(httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<DemandeRestaurationDTO>> GetAllDemandeRestaurations()
    {
        var response = await GetWithCredentialsAsync("DemandeRestauration");
        response.EnsureSuccessStatusCode();
        var demandes = await response.Content.ReadFromJsonAsync<List<DemandeRestaurationDTO>>();
        return demandes ?? new List<DemandeRestaurationDTO>();
    }

    public async Task<DemandeRestaurationDetailDTO> GetDemandeRestaurationDetail(int demandeRestaurationId)
    {
        var response = await GetWithCredentialsAsync($"DemandeRestauration/{demandeRestaurationId}");
        response.EnsureSuccessStatusCode();
        var demande = await response.Content.ReadFromJsonAsync<DemandeRestaurationDetailDTO>();
        return demande;
    }

    public async Task<APIResponse<DemandeRestaurationDetailDTO>> AddDemandeRestauration(string demande)
{
    try
    {
        var body = JsonContent.Create(demande);
        var response = await PostWithCredentialsAsync("DemandeRestauration", body);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(errorContent))
            {
                try
                {
                    var errorObj = JsonSerializer.Deserialize<Dictionary<string, object>>(errorContent);
                    if (errorObj != null && errorObj.ContainsKey("message"))
                    {
                        return APIResponse<DemandeRestaurationDetailDTO>.ErrorResponse(errorObj["message"].ToString());
                    }
                }
                catch
                {
                    return APIResponse<DemandeRestaurationDetailDTO>.ErrorResponse(errorContent);
                }
            }
            return response.StatusCode switch
            {
                System.Net.HttpStatusCode.BadRequest => 
                    APIResponse<DemandeRestaurationDetailDTO>.ErrorResponse(errorContent ?? "Requête invalide"),
                System.Net.HttpStatusCode.Unauthorized => 
                    APIResponse<DemandeRestaurationDetailDTO>.ErrorResponse("Non autorisé"),
                System.Net.HttpStatusCode.NotFound => 
                    APIResponse<DemandeRestaurationDetailDTO>.ErrorResponse("Ressource introuvable"),
                _ => 
                    APIResponse<DemandeRestaurationDetailDTO>.ErrorResponse($"Erreur {(int)response.StatusCode}")
            };
        }
        
        var result = await response.Content.ReadFromJsonAsync<DemandeRestaurationDetailDTO>();
        return APIResponse<DemandeRestaurationDetailDTO>.SuccessResponse(result);
    }
    catch (HttpRequestException ex)
    {
        return APIResponse<DemandeRestaurationDetailDTO>.ErrorResponse(
            "Erreur de connexion au serveur. Vérifiez votre connexion internet.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erreur inattendue: {ex.Message}");
        return APIResponse<DemandeRestaurationDetailDTO>.ErrorResponse(
            "Une erreur inattendue s'est produite. Veuillez réessayer.");
    }
}
}