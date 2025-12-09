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
    public override async Task UpdateAsync(Utilisateur entityToUpdate, Utilisateur entity)
    {
        // Récupérer l'entité depuis le contexte si elle est déjà trackée
        var tracked = _context.Set<Utilisateur>().Local
            .FirstOrDefault(e => e.UtilisateurId == entityToUpdate.UtilisateurId);

        if (tracked == null)
        {
            // Si pas trackée, attacher l'entité reçue
            _context.Set<Utilisateur>().Attach(entityToUpdate);
            tracked = entityToUpdate;
        }

        // IMPORTANT : Marquer manuellement seulement les propriétés modifiées
        var entry = _context.Entry(tracked);
    
        // Copier uniquement les propriétés non-null du DTO (déjà mappées dans tracked)
        // On ne touche PAS à StatutId et RoleId
        if (!string.IsNullOrEmpty(entity.Email))
            entry.Property(u => u.Email).IsModified = true;
    
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

        // NE JAMAIS marquer StatutId et RoleId comme modifiés
        entry.Property(u => u.StatutId).IsModified = false;
        entry.Property(u => u.RoleId).IsModified = false;

        await _context.SaveChangesAsync();
    }
}