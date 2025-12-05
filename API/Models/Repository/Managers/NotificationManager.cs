using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class NotificationManager : GenericCRUDManager<Notification>, INotificationRepository
{
    public NotificationManager(Clothes2UDbContext context) : base(context)
    {
    }

    /// <summary>
    /// Base query avec toutes les relations nécessaires pour AutoMapper
    /// </summary>
    private IQueryable<Notification> BaseNotificationQuery()
    {
        return _context.Notifications
            .Include(n => n.NotificationType)
            .Include(n => n.NotificationAdmins)
            .Include(n => n.NotificationAvertissements)
            .Include(n => n.NotificationMessages)
            .ThenInclude(nm => nm.Message)
            .ThenInclude(m => m.Utilisateur)  // <-- essentiel pour récupérer l'auteur
            .Include(n => n.NotificationNouvellesAnnonces)
            .ThenInclude(nna => nna.Annonce)
            .ThenInclude(a => a.Utilisateur)
            .Include(n => n.NotificationModifications)
            .ThenInclude(nm => nm.Annonce)
            .ThenInclude(a => a.Utilisateur)
            .AsSplitQuery();
    }


    /// <summary>
    /// Récupère toutes les notifications d’un utilisateur
    /// </summary>
    public async Task<IEnumerable<Notification>> GetByUserId(int userId)
    {
        return await BaseNotificationQuery()
            .Where(n => n.UtilisateurId == userId)
            .OrderByDescending(n => n.DateCreation)
            .ToListAsync();
    }

    public Task<int> GetNotificationsUnreadCountByUserId(int userId)
    {
        return BaseNotificationQuery()
            .Where(n => n.EstLu == false && n.UtilisateurId == userId)
            .CountAsync();
    }

    public async Task MarkAsRead(int userId)
    {
        var notifications = BaseNotificationQuery();
        foreach (var notification in notifications)
        {
            notification.EstLu = true;
            await _context.SaveChangesAsync();
        }
    }
}
