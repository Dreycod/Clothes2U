using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public class NotificationPropositionManager : GenericCRUDManager<NotificationProposition>,INotificationPropositionRepository
{
    public NotificationPropositionManager(Clothes2UDbContext context) : base(context){}
}