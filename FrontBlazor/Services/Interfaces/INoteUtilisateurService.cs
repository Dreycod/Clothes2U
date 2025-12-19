using Shared.DTO;
using Shared.DTO.NoteUtilisateur;

namespace FrontBlazor.Services.GenericIServices;

public interface INoteUtilisateurService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
    Task<List<TEntity>?> GetAllNotesByUtilisateurId(int utilisateurId);
    Task<HttpResponseMessage> AddNoteUtilisateur(NoteUtilisateurCreateDTO noteUtilisateurCreate);
}