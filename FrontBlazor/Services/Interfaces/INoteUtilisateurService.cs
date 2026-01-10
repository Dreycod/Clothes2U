using Shared.DTO;
using Shared.DTO.NoteUtilisateur;
using FrontBlazor.Services.Interfaces.GenericIServices;

namespace FrontBlazor.Services.Interfaces;

public interface INoteUtilisateurService : IReadableService<NoteUtilisateurDetailDTO>, IWritableService<NoteUtilisateurDTO>
{
    Task<List<NoteUtilisateurDetailDTO>?> GetAllNotesByUtilisateurId(int utilisateurId);
    Task<HttpResponseMessage> AddNoteUtilisateur(NoteUtilisateurCreateDTO noteUtilisateurCreate);
}