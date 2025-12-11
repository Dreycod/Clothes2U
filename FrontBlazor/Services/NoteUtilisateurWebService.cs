using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;
using System.Net.Http.Json;

namespace FrontBlazor.Services
{
    public class NoteUtilisateurWebService : WritableService<NoteUtilisateur>, INoteUtilisateurService<NoteUtilisateur>
    {
        public NoteUtilisateurWebService(HttpClient httpClient) : base(httpClient) {}

        public async Task AddNoteUtilisateur(NoteUtilisateurCreate noteUtilisateurCreate)
        {
            var body = JsonContent.Create(noteUtilisateurCreate);

            var response = await PostWithCredentialsAsync("NoteUtilisateur", body);
        
        }

        public async Task<List<NoteUtilisateur>?> GetAllNotesByUtilisateurId(int utilisateurId)
        {
            return await _httpClient.GetFromJsonAsync<List<NoteUtilisateur>?>($"NoteUtilisateur/User/{utilisateurId}");
        }

        public async Task<NoteUtilisateur?> GetByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<NoteUtilisateur>($"NoteUtilisateur/{id}");
        }

        // delete est dans WritableService
    }
}
