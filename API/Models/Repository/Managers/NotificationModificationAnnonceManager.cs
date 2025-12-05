using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class NotificationModificationAnnonceManager : GenericCRUDManager<NotificationModificationAnnonce>, INotificationModificationAnnonceRepository
{
    public NotificationModificationAnnonceManager(Clothes2UDbContext context) : base(context)
    {
    }

    public async Task<NotificationModificationAnnonce?> GetByNotificationIdAsync(int notificationId)
    {
        return await _context.Set<NotificationModificationAnnonce>()
            .Include(nma => nma.Annonce)
            .ThenInclude(a => a.Utilisateur)
            .Include(nma => nma.Annonce)
            .ThenInclude(a => a.Photos)
            .Include(nma => nma.LaNotification)
            .FirstOrDefaultAsync(nma => nma.NotificationId == notificationId);
    }
}