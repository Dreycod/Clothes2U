using API.Models.EntityFramework;

namespace API.Models.Repository;

public interface INotificationRepository : IDataRepository<Notification, int>
{
    Task<IEnumerable<Notification>> GetByUtilisateurIdAsync(int utilisateurId);
    Task<IEnumerable<Notification>> GetUnreadByUtilisateurIdAsync(int utilisateurId);
    public Task<IEnumerable<Notification>> GetByUserId(int userId);
}

public interface INotificationMessageRepository : IDataRepository<NotificationMessage, int>
{
}

public interface INotificationNouvelleAnnonceRepository : IDataRepository<NotificationNouvelleAnnonce, int>
{
}

public interface INotificationModificationAnnonceRepository : IDataRepository<NotificationModificationAnnonce, int>
{
}