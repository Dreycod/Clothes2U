using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class NotificationManager : GenericCRUDManager<Notification>, INotificationRepository
{
    public NotificationManager(Clothes2UDbContext context) : base(context)
    {
    }
    private IQueryable<Notification> BaseNotificationQuery()
    {
        return _context.Notifications
            .Include(n => n.NotificationType)
            .Include(n => n.NotificationAdmins)
            .Include(n => n.NotificationAvertissements)
            .Include(n => n.NotificationMessages)
                .ThenInclude(nm => nm.Message)
                    .ThenInclude(m => m.Conversation)  
            .Include(n => n.NotificationMessages)
                .ThenInclude(nm => nm.Message)
                    .ThenInclude(m => m.Utilisateur)
            .Include(n => n.NotificationNouvellesAnnonces)
                .ThenInclude(nna => nna.Annonce)
                    .ThenInclude(a => a.Utilisateur)
            .Include(n => n.NotificationNouvellesAnnonces)
                .ThenInclude(nna => nna.Annonce)
                    .ThenInclude(a => a.Photos)
                        .ThenInclude(p => p.Photo)
            .Include(n => n.NotificationModifications)
                .ThenInclude(nm => nm.Annonce)
                    .ThenInclude(a => a.Utilisateur)
            .Include(n => n.NotificationModifications)
                .ThenInclude(nm => nm.Annonce)
                    .ThenInclude(a => a.Photos)
                        .ThenInclude(p => p.Photo)
            
            .AsSplitQuery();
    }
    public async Task<IEnumerable<Notification>> GetByUserId(int userId)
    {
        return await BaseNotificationQuery()
            .Where(n => n.UtilisateurId == userId)
            .OrderByDescending(n => n.DateCreation)
            .ToListAsync();
    }
    public async Task<int> GetNotificationsUnreadCountByUserId(int userId)
    {
        return await _context.Notifications
            .Where(n => n.EstLu == false && n.UtilisateurId == userId)
            .CountAsync();
    }
    public async Task MarkAsRead(int userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.UtilisateurId == userId && n.EstLu == false)
            .ToListAsync();
        
        foreach (var notification in notifications)
        {
            notification.EstLu = true;
        }
        
        await _context.SaveChangesAsync();
    }

    public async Task CreateNotificationAvertissement(NotificationAvertissement notification)
    {
        await  _context.NotificationAvertissements.AddAsync(notification);
        await _context.SaveChangesAsync();
    }
}