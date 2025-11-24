using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class NotificationManager :  GenericCRUDManager<Notification>,INotificationRepository<Notification>
{
    public NotificationManager(Clothes2UDbContext context) : base(context){}
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

    
}