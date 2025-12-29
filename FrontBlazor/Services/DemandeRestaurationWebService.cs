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
        Console.WriteLine("RESULT : " + demande);
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
    public async Task<APIResponse<string>> SubmitDecisionDemande(DecisionDemandeRestaurationDTO decisionDemandeRestauration)
    {
        Console.WriteLine("ca part");
        try
        {
            var body = JsonContent.Create(decisionDemandeRestauration);
            var response = await PostWithCredentialsAsync("DemandeRestauration/decisionDemandeRestauration", body);
        
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine("RESULT : " + response.Content.ReadAsStringAsync().Result);
                var errorContent = await response.Content.ReadAsStringAsync();
                return response.StatusCode switch
                {
                    System.Net.HttpStatusCode.BadRequest => 
                        APIResponse<string>.ErrorResponse(errorContent ?? "Cette demande a déjà été traitée ou la requête est invalide"),
                    System.Net.HttpStatusCode.NotFound => 
                        APIResponse<string>.ErrorResponse(errorContent ?? "Demande ou utilisateur introuvable"),
                    System.Net.HttpStatusCode.Unauthorized => 
                        APIResponse<string>.ErrorResponse("Non autorisé - vous devez être Admin ou Modérateur"),
                    System.Net.HttpStatusCode.InternalServerError =>
                        APIResponse<string>.ErrorResponse("Une erreur est survenue lors du traitement de la demande"),
                    _ => 
                        APIResponse<string>.ErrorResponse($"Erreur {(int)response.StatusCode}: {errorContent}")
                };
            }
            return APIResponse<string>.SuccessResponse("Décision soumise avec succès");
        }
        catch (Exception ex)
        {
            return APIResponse<string>.ErrorResponse($"Erreur de communication: {ex.Message}");
        }
    }
}