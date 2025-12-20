using Shared.DTO;
using Shared.DTO.NoteUtilisateur;

namespace FrontBlazor.Services.GenericIServices;

public interface INoteUtilisateurService : IReadableService<NoteUtilisateurDetailDTO>, IWritableService<NoteUtilisateurDTO>
{
    Task<List<NoteUtilisateurDetailDTO>?> GetAllNotesByUtilisateurId(int utilisateurId);
    Task<HttpResponseMessage> AddNoteUtilisateur(NoteUtilisateurCreateDTO noteUtilisateurCreate);
}