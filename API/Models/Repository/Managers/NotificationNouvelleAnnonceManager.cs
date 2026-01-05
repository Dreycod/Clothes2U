using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class NotificationNouvelleAnnonceManager : GenericCRUDManager<NotificationNouvelleAnnonce>, IDataRepository<NotificationNouvelleAnnonce, int>
{
    public NotificationNouvelleAnnonceManager(Clothes2UDbContext context) : base(context) { }
}