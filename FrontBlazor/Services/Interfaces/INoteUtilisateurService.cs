using Shared.DTO;
using Shared.DTO.NoteUtilisateur;

namespace FrontBlazor.Services.GenericIServices;

public interface INoteUtilisateurService : IReadableService<NoteUtilisateurDTO>, IWritableService<NoteUtilisateurDTO>
{
    Task<List<NoteUtilisateurDTO>?> GetAllNotesByUtilisateurId(int utilisateurId);
    Task<HttpResponseMessage> AddNoteUtilisateur(NoteUtilisateurCreateDTO noteUtilisateurCreate);
}