using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public class NotificationAchatManager : GenericCRUDManager<NotificationAchatAnnonce>, IDataRepository<NotificationAchatAnnonce, int>
{
    public NotificationAchatManager(Clothes2UDbContext context) : base(context){}
}