using Shared.DTO;
using Shared.DTO.NoteUtilisateur;
using FrontBlazor.Services.Interfaces.GenericIServices;
using Shared;

namespace FrontBlazor.Services.Interfaces;

public interface INoteUtilisateurService : IReadableService<NoteUtilisateurDetailDTO>, IWritableService<NoteUtilisateurDTO>
{
    Task<List<NoteUtilisateurDetailDTO>?> GetNotesByUtilisateurId(int utilisateurId, int page = 1, int pageSize = 5);
    Task<APIResponse<object>> AddNoteUtilisateur(NoteUtilisateurCreateDTO noteUtilisateurCreate);
}