using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class UtilisateurManager : GenericCRUDManager<Utilisateur>, IUtilisateurRepository
{
    public UtilisateurManager(Clothes2UDbContext context) : base(context)
    {
    }
    
    private IQueryable<Utilisateur> BaseUtilisateurQuery()
    {
        return _context.Utilisateurs
            .Include(u => u.PhotoProfil)
            .Include(u => u.Statut)
            .Include(u => u.Role)
            .Include(u => u.Adresse)
            .Include(u => u.Abonnes)              
            .Include(u => u.Abonnements)          
            .Include(u => u.NotesCible)           
            .Include(u => u.Annonces)             
            .ThenInclude(a => a.Photos)
            .ThenInclude(pa => pa.Photo)
            .AsSplitQuery();
    }

    public override async Task<Utilisateur?> GetByIdAsync(int id)
    {
        return await BaseUtilisateurQuery()
            .FirstOrDefaultAsync(u => u.UtilisateurId == id);
    }

    public async Task UpdatePassword(Utilisateur utilisateur, string newPassword)
    {
        utilisateur.Password = newPassword;
        await _context.SaveChangesAsync();
    }
}