using Shared.DTO;
using Shared.DTO.NoteUtilisateur;
using FrontBlazor.Services.GenericService;
using FrontBlazor.Services.Interfaces;
using System.Net.Http.Json;
using Shared;

namespace FrontBlazor.Services
{
    public class NoteUtilisateurWebService : WritableService<NoteUtilisateurDTO>, INoteUtilisateurService
    {
        public NoteUtilisateurWebService(HttpClient httpClient) : base(httpClient) {}

        public async Task<APIResponse<object>> AddNoteUtilisateur(NoteUtilisateurCreateDTO noteUtilisateurCreate)
        {
            var body = JsonContent.Create(noteUtilisateurCreate);

            var response = await PostWithCredentialsAsync("NoteUtilisateur", body);
            var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse<object>>();

            if (apiResponse == null)
            {
                return APIResponse<object>.ErrorResponse("Réponse serveur invalide");
            }

            return apiResponse;
        }

        public async Task<List<NoteUtilisateurDetailDTO>?> GetNotesByUtilisateurId(int utilisateurId, int page = 1, int pageSize = 5)
        {
            var response = await GetWithCredentialsAsync($"NoteUtilisateur/User/{utilisateurId}?page={page}&pageSize={pageSize}");
            response.EnsureSuccessStatusCode();
            var avis = await response.Content.ReadFromJsonAsync<List<NoteUtilisateurDetailDTO>>();
            return avis ?? new List<NoteUtilisateurDetailDTO>();
        }

        public async Task<NoteUtilisateurDetailDTO?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<NoteUtilisateurDetailDTO>($"NoteUtilisateur/{id}");
        }
    }
}
