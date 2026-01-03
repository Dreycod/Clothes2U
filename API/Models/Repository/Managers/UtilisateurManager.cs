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
            .Include(u => u.Role)
            .Include(u => u.Abonnements)          
            .Include(u => u.NotesCible)           
            .Include(u => u.Annonces)             
            .ThenInclude(a => a.Photos)
            .ThenInclude(pa => pa.Photo)
            .AsSplitQuery();
    }

    public async Task<Utilisateur> GetUtilisateurByLogin(string login)
    {
        return await  BaseUtilisateurQuery().FirstOrDefaultAsync(u => u.Login == login);
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
    public override async Task UpdateAsync(Utilisateur entity)
    {
        Utilisateur entityToUpdate = await GetByIdAsync(entity.UtilisateurId) 
            ?? throw new ArgumentException($"Utilisateur with id {entity.UtilisateurId} not found");
        var tracked = _context.Set<Utilisateur>().Local
            .FirstOrDefault(e => e.UtilisateurId == entityToUpdate.UtilisateurId);

        if (tracked == null)
        {
            _context.Set<Utilisateur>().Attach(entityToUpdate);
            tracked = entityToUpdate;
        }
        var entry = _context.Entry(tracked);

        if (!string.IsNullOrEmpty(entity.Email) && entity.Email != entityToUpdate.Email)
        {
            tracked.Email = entity.Email;
            tracked.ValidEmail = false;
            entry.Property(u => u.Email).IsModified = true;
            entry.Property(u => u.ValidEmail).IsModified = true;
        }

        if (entity.Telephone != null)
            entry.Property(u => u.Telephone).IsModified = true;
    
        if (!string.IsNullOrEmpty(entity.Login))
            entry.Property(u => u.Login).IsModified = true;
    
        if (!string.IsNullOrEmpty(entity.Description))
            entry.Property(u => u.Description).IsModified = true;
    
        if (entity.AdresseId.HasValue)
            entry.Property(u => u.AdresseId).IsModified = true;
    
        if (entity.PhotoId.HasValue)
            entry.Property(u => u.PhotoId).IsModified = true;
        tracked.StatutId = entity.StatutId;
        entry.Property(u => u.StatutId).IsModified = true;
        entry.Property(u => u.RoleId).IsModified = false;

        await _context.SaveChangesAsync();
    }

    public async Task<int> GetSuspendUserCount()
    {
        return await _context.Utilisateurs.Where(u => u.Statut.StatutLibelle == "Suspendu").CountAsync();
    }
}