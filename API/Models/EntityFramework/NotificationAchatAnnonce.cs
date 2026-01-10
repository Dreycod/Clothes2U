using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_e_notification_achat_notach")]
public class NotificationAchatAnnonce : IEntity
{
    [Key]
    [Column("notach_id")]
    public int NotificationAchatAnnonceId { get; set; }
    
    [Column("notach_annonce_id")]
    public int AnnonceId { get; set; }
    
    [Column("notach_notification_id")]
    public int NotificationId { get; set; }
    
    //relations avec d'autres tables : 
    
    [ForeignKey(nameof(AnnonceId))]
    [InverseProperty(nameof(Annonce.NotificationsAchatAnnonces))]
    public virtual Annonce Annonce { get; set; } = null!;
    
    [ForeignKey(nameof(NotificationId))]
    [InverseProperty(nameof(Notification.NotificationAchats))]
    public virtual Notification LaNotification { get; set; } = null!;
    
    public int GetId() => NotificationAchatAnnonceId;
}