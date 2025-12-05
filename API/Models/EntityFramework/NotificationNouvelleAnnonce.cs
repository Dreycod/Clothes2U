using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_notification_nouvelle_annonce_notnou")]
public class NotificationNouvelleAnnonce : IEntity
{
    [Key]
    [Column("notnou_id")]
    public int NotificationNouvelleAnnonceId { get; set; }
    
    [Column("notnou_annonce_id")]
    public int AnnonceId { get; set; }
    
    [Column("notnou_notification_id")]
    public int NotificationId { get; set; }
    
    //relation avec les autres tables : 
    
    [ForeignKey(nameof(AnnonceId))]
    [InverseProperty(nameof(Annonce.NotificationsNouvelleAnnonces))]
    public virtual Annonce Annonce { get; set; } = null!;

    [ForeignKey(nameof(NotificationId))]
    [InverseProperty(nameof(Notification.NotificationNouvellesAnnonces))]
    public virtual Notification LaNotification { get; set; } = null!;
    
    public int GetId() => NotificationNouvelleAnnonceId;
}