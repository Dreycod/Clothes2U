using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Shared.DTO;

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
            .Include(u => u.Adresses)
            .Include(u => u.Abonnes)  
            .Include(u => u.Role)
            .Include(u => u.Abonnements)          
            .Include(u => u.NotesCible)           
            .Include(u => u.Annonces)             
            .ThenInclude(a => a.Photos)
            .ThenInclude(pa => pa.Photo)
            .AsSplitQuery();
    }

    public async Task<Utilisateur?> GetUtilisateurByLogin(string login)
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
        var entityToUpdate = await _context.Utilisateurs
            .FirstOrDefaultAsync(u => u.UtilisateurId == entity.UtilisateurId)
            ?? throw new ArgumentException($"Utilisateur with id {entity.UtilisateurId} not found");
        var entry = _context.Entry(entityToUpdate);
        if (!string.IsNullOrEmpty(entity.Email) && entity.Email != entityToUpdate.Email)
        {
            entityToUpdate.Email = entity.Email;
            entityToUpdate.ValidEmail = false;
            entry.Property(u => u.Email).IsModified = true;
            entry.Property(u => u.ValidEmail).IsModified = true;
        }
        if (!string.IsNullOrEmpty(entity.Telephone) && entity.Telephone != entityToUpdate.Telephone)
        {
            entityToUpdate.Telephone = entity.Telephone;
            entry.Property(u => u.Telephone).IsModified = true;
        }
        if (!string.IsNullOrEmpty(entity.Login) && entity.Login != entityToUpdate.Login)
        {
            entityToUpdate.Login = entity.Login;
            entry.Property(u => u.Login).IsModified = true;
        }
        if (!string.IsNullOrEmpty(entity.Description) && entity.Description != entityToUpdate.Description)
        {
            entityToUpdate.Description = entity.Description;
            entry.Property(u => u.Description).IsModified = true;
        }
        if (entity.PhotoId.HasValue && entity.PhotoId != entityToUpdate.PhotoId)
        {
            entityToUpdate.PhotoId = entity.PhotoId;
            entry.Property(u => u.PhotoId).IsModified = true;
        }
        if (entity.StatutId != entityToUpdate.StatutId)
        {
            entityToUpdate.StatutId = entity.StatutId;
            entry.Property(u => u.StatutId).IsModified = true;
        }
        entityToUpdate.PreferenceNotifMail = entity.PreferenceNotifMail;
        entry.Property(u => u.PreferenceNotifMail).IsModified = true;

        entityToUpdate.PreferenceTheme = entity.PreferenceTheme;
        entry.Property(u => u.PreferenceTheme).IsModified = true;

        entityToUpdate.PreferenceCookies = entity.PreferenceCookies;
        entry.Property(u => u.PreferenceCookies).IsModified = true;
        entry.Property(u => u.RoleId).IsModified = false;

        await _context.SaveChangesAsync();
    }

    public async Task<int> GetSuspendUserCount()
    {
        return await _context.Utilisateurs.Where(u => u.Statut.StatutLibelle == "Suspendu").CountAsync();
    }

    public async Task<Utilisateur?> GetUtilisateurByEmail(string email)
    {
        return await BaseUtilisateurQuery()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task BanUser(int userId)
    {
        Utilisateur user = _context.Utilisateurs.Where(u => u.UtilisateurId == userId).FirstOrDefault();
        user.StatutId = 3;
        await _context.SaveChangesAsync();
    }
    public async Task SuspendUser(int userId)
    {
        Utilisateur user = _context.Utilisateurs.Where(u => u.UtilisateurId == userId).FirstOrDefault();
        user.StatutId = 2;
        await _context.SaveChangesAsync();
    }
}