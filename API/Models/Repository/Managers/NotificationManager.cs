using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class NotificationManager : GenericCRUDManager<Notification>, INotificationRepository
{
    public NotificationManager(Clothes2UDbContext context) : base(context)
    {
    }
    private IQueryable<Notification> BaseAnnonceQuery()
    {
        return _context.Notifications
            .Include(n => n.NotificationAdmins)
            .Include(n => n.NotificationAvertissements)
            .Include(n =>n.NotificationMessages)
            .Include(n => n.NotificationModifications)
            .Include(n => n.NotificationNouvellesAnnonces)
            .AsSplitQuery();
    }
    public async Task<IEnumerable<Notification>> GetByUserId(int userId)
    {
        return await BaseAnnonceQuery().Where(n => n.UtilisateurId == userId).ToListAsync();
    }
    public async Task<IEnumerable<Notification>> GetByUtilisateurIdAsync(int utilisateurId)
    {
        return await _context.Notifications
            .Include(n => n.NotificationType)
            .Include(n => n.NotificationMessages)
            .ThenInclude(nm => nm.Message)
            .Include(n => n.NotificationNouvellesAnnonces)
            .ThenInclude(nna => nna.Annonce)
            .Include(n => n.NotificationModifications)
            .ThenInclude(nm => nm.Annonce)
            .Where(n => n.UtilisateurId == utilisateurId)
            .OrderByDescending(n => n.DateCreation)
            .ToListAsync();
    }

    public async Task<IEnumerable<Notification>> GetUnreadByUtilisateurIdAsync(int utilisateurId)
    {
        return await _context.Notifications
            .Include(n => n.NotificationType)
            .Include(n => n.NotificationMessages)
            .ThenInclude(nm => nm.Message)
            .Include(n => n.NotificationNouvellesAnnonces)
            .ThenInclude(nna => nna.Annonce)
            .Include(n => n.NotificationModifications)
            .ThenInclude(nm => nm.Annonce)
            .Where(n => n.UtilisateurId == utilisateurId && !n.EstLu)
            .OrderByDescending(n => n.DateCreation)
            .ToListAsync();
    }
}