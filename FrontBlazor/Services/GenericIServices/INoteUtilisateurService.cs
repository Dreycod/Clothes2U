using FrontBlazor.Models;

namespace FrontBlazor.Services.GenericIServices;

public interface INoteUtilisateurService<TEntity> : IReadableService<TEntity>, IWritableService<TEntity> where TEntity : class
{
    Task<List<TEntity>?> GetAllNotesByUtilisateurId(int utilisateurId);
    Task AddNoteUtilisateur(NoteUtilisateurCreate noteUtilisateurCreate);
}