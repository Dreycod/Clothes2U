using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public class NotificationPropositionManager : GenericCRUDManager<NotificationProposition>,IDataRepository<NotificationProposition, int>
{
    public NotificationPropositionManager(Clothes2UDbContext context) : base(context){}
}