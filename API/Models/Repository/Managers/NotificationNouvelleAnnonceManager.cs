using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class NotificationNouvelleAnnonceManager : GenericCRUDManager<NotificationNouvelleAnnonce>, INotificationNouvelleAnnonceRepository
{
    public NotificationNouvelleAnnonceManager(Clothes2UDbContext context) : base(context)
    {
    }

    public async Task<NotificationNouvelleAnnonce?> GetByNotificationIdAsync(int notificationId)
    {
        return await _context.Set<NotificationNouvelleAnnonce>()
            .Include(nna => nna.Annonce)
            .ThenInclude(a => a.Utilisateur)
            .Include(nna => nna.Annonce)
            .ThenInclude(a => a.Photos)
            .Include(nna => nna.LaNotification)
            .FirstOrDefaultAsync(nna => nna.NotificationId == notificationId);
    }
}