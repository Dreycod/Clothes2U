using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public class NotificationCommercialManager : GenericCRUDManager<NotificationCommercial>, IDataRepository<NotificationCommercial, int>
{
    public NotificationCommercialManager(Clothes2UDbContext context) : base(context)
    {
    }
}
