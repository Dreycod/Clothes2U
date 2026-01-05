using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class NotificationMessageManager : GenericCRUDManager<NotificationMessage>, IDataRepository<NotificationMessage, int>
{
    public NotificationMessageManager(Clothes2UDbContext context) : base(context) { }
}