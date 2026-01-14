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
                    .ThenInclude(m => m.MessageEstPayee)
            .Include(n => n.NotificationMessages)
                .ThenInclude(nm => nm.Message)
                    .ThenInclude(m => m.MessageEnvoieColis)
            .Include(n => n.NotificationMessages)
                .ThenInclude(nm => nm.Message)
                    .ThenInclude(m => m.MessageEstRecu)
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
            .Include(n => n.NotificationProposition)
                .ThenInclude(np => np.MessageDemande)
                    .ThenInclude(nm => nm.Offre)
            .Include(n => n.NotificationProposition)
                .ThenInclude(np => np.MessageDemande)
                    .ThenInclude(nm => nm.Message)
                        .ThenInclude(n => n.Conversation)
                            .ThenInclude(c => c.LAnnonce)
            .Include(n => n.NotificationProposition)
                .ThenInclude(np => np.MessageDemande)
                    .ThenInclude(nm => nm.Message)
                        .ThenInclude(n => n.Utilisateur)
            .Include(n => n.NotificationAchats)
                .ThenInclude(nm => nm.Annonce)
                    .ThenInclude(a => a.Utilisateur)
            .AsSplitQuery();
    }

    public async Task<IEnumerable<Notification>> GetReadNotifications()
    {
        return await BaseNotificationQuery().Where(n => n.EstLu).ToListAsync();
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

    public async Task DeleteMessageNotificationByConversationId(int id, int userId)
    {
        var notifications = await _context.Notifications.Where(n => 
            (n.NotificationMessages != null
             && n.NotificationMessages.Message.ConversationId == id
             && n.UtilisateurId == userId)
            ||
            (n.NotificationProposition != null  
             && n.NotificationProposition.MessageDemande.Message.ConversationId == id  
             && n.UtilisateurId == userId)
        ).Distinct().ToListAsync();
        _context.Notifications.RemoveRange(notifications);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteNotificationAnnonceForUser(int annonceId, int userId)
    {
        var notifNouvellesAnnonces = await _context.NotificationNouvelleAnnonces
            .Where(nna => nna.AnnonceId == annonceId && nna.LaNotification.UtilisateurId == userId)
            .Select(nna => nna.LaNotification)
            .ToListAsync();
    
        var notifModifications = await _context.NotificationModificationAnnonces
            .Where(nm => nm.AnnonceId == annonceId && nm.LaNotification.UtilisateurId == userId)
            .Select(nm => nm.LaNotification)
            .ToListAsync();
    
        var allNotifications = notifNouvellesAnnonces.Concat(notifModifications).Distinct();
    
        _context.Notifications.RemoveRange(allNotifications);
        await _context.SaveChangesAsync();
    }
}