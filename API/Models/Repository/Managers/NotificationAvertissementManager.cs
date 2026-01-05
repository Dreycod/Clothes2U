using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public class NotificationAvertissementManager : GenericCRUDManager<NotificationAvertissement>, IDataRepository<NotificationAvertissement, int>
{
    public NotificationAvertissementManager(Clothes2UDbContext context) : base(context)
    {
    }
}