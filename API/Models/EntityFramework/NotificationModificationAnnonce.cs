using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_notification_modifiaction_notmod")]
public class NotificationModificationAnnonce 
{
    [Key]
    [Column("notmod_id")]
    public int NotificationModificationId { get; set; }
    
    [Column("notmod_annonce_id")]
    public int AnnonceId { get; set; }
    
    [Column("notmod_notification_id")]
    public int NotificationId { get; set; }
    
    //relations avec d'autres tables : 
    
    [ForeignKey(nameof(AnnonceId))]
    [InverseProperty(nameof(Annonce.NotificationsModificationAnnonces))]
    public virtual Annonce Annonce { get; set; } = null!;
    
    [ForeignKey(nameof(NotificationId))]
    [InverseProperty(nameof(Notification.NotificationModifications))]
    public virtual Notification LaNotification { get; set; } = null!;
}