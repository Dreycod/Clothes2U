using Shared.DTO;
using Shared.DTO.NoteUtilisateur;
using FrontBlazor.Services.GenericService;
using FrontBlazor.Services.Interfaces;
using System.Net.Http.Json;

namespace FrontBlazor.Services
{
    public class NoteUtilisateurWebService : WritableService<NoteUtilisateurDTO>, INoteUtilisateurService
    {
        public NoteUtilisateurWebService(HttpClient httpClient) : base(httpClient) {}

        public async Task<HttpResponseMessage> AddNoteUtilisateur(NoteUtilisateurCreateDTO noteUtilisateurCreate)
        {
            var body = JsonContent.Create(noteUtilisateurCreate);

            var response = await PostWithCredentialsAsync("NoteUtilisateur", body);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            return response;
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
