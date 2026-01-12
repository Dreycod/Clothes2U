using FrontBlazor.Services.GenericService;
using FrontBlazor.Services.Interfaces;
using Shared.DTO.SupportTicket;
using System.Net.Http.Json;

namespace FrontBlazor.Services.WebService
{
    public class SupportWebService : BaseGenericService, ISupportService
    {
        public SupportWebService(HttpClient httpClient) : base(httpClient) { }

        // ======================
        // USER
        // ======================
        public async Task CreateTicketAsync(SupportTicketCreateDTO dto)
        {
            var content = JsonContent.Create(dto);

            var response = await PostWithCredentialsAsync("support", content);
            response.EnsureSuccessStatusCode();
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
    }
}
