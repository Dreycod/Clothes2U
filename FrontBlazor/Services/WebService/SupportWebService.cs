using FrontBlazor.Services.GenericService;
using FrontBlazor.Services.Interfaces;
using Shared.DTO.SupportTicket;
using System.Net.Http.Json;
using System.Text.Json;
using Shared;

namespace FrontBlazor.Services.WebService
{
    public class SupportWebService : BaseGenericService, ISupportService
    {
        public SupportWebService(HttpClient httpClient) : base(httpClient) { }

        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        // ======================
        // USER
        // ======================
        public async Task<APIResponse<SupportTicketCreateDTO>> CreateTicketAsync(SupportTicketCreateDTO dto)
        {
            try
            {
                var content = JsonContent.Create(dto);
                var response = await PostWithCredentialsAsync("support", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                
                var apiResponse = JsonSerializer.Deserialize<APIResponse<SupportTicketCreateDTO>>(
                    responseContent, 
                    _jsonOptions
                );
                
                return apiResponse ?? APIResponse<SupportTicketCreateDTO>.ErrorResponse("Réponse invalide du serveur");
            }
            catch (Exception ex)
            {
                return APIResponse<SupportTicketCreateDTO>.ErrorResponse($"Erreur: {ex.Message}");
            }
        }

        // ======================
        // ADMIN
        // ======================
        public async Task<List<SupportTicketViewDTO>> GetOpenTicketsAsync()
        {
            var response = await GetWithCredentialsAsync("support");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<List<SupportTicketViewDTO>>();
        }

        public async Task ReplyAsync(SupportTicketReplyDTO dto)
        {
            var content = JsonContent.Create(dto);

            var response = await PostWithCredentialsAsync("support/reply", content);
            response.EnsureSuccessStatusCode();
        }

        public async Task<TicketDetailViewDTO> GetTicketById(int id)
        {
            try
            {
                var response = await GetWithCredentialsAsync($"support/id/{id}");
                response.EnsureSuccessStatusCode();
                var ticket = await response.Content.ReadFromJsonAsync<TicketDetailViewDTO>();
                return ticket ?? null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task CloseTicket(int id)
        {
            var response = await PutWithCredentialsAsync($"support/closeTicket/{id}");
    
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Erreur lors de la fermeture du ticket: {response.StatusCode} - {errorContent}");
            }
        }
    }
}
