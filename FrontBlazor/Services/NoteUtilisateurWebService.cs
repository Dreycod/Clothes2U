using Shared.DTO;
using Shared.DTO.NoteUtilisateur;
using FrontBlazor.Services.GenericIServices;
using System.Net.Http.Json;

namespace FrontBlazor.Services
{
    public class NoteUtilisateurWebService : WritableService<NoteUtilisateurDetailDTO>, INoteUtilisateurService<NoteUtilisateurDetailDTO>
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

        public async Task<List<NoteUtilisateurDetailDTO>?> GetAllNotesByUtilisateurId(int utilisateurId)
        {
            return await _httpClient.GetFromJsonAsync<List<NoteUtilisateurDetailDTO>?>($"NoteUtilisateur/User/{utilisateurId}");
        }

        public async Task<NoteUtilisateurDetailDTO?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<NoteUtilisateurDetailDTO>($"NoteUtilisateur/{id}");
        }

        // delete est dans WritableService
    }
}
