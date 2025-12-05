using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class NotificationMessageManager : GenericCRUDManager<NotificationMessage>, INotificationMessageRepository
{
    public NotificationMessageManager(Clothes2UDbContext context) : base(context)
    {
    }

    public async Task<NotificationMessage?> GetByNotificationIdAsync(int notificationId)
    {
        return await _context.Set<NotificationMessage>()
            .Include(nm => nm.Message)
            .Include(nm => nm.LaNotification)
            .FirstOrDefaultAsync(nm => nm.NotificationId == notificationId);
    }
}