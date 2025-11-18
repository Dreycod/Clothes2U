using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_e_notification_admin_notadm")]
public class NotificationAdmin
{
    [Key]
    [Column("notadm_id")]
    public int NotificationAdminId { get; set; }
    
    [Column("notadm_admin_text")]
    public String AdminText { get; set; }
    
    
    // relation avec la table notification
    [Column("notadm_notification_id")]
    public int NotificationId { get; set; }

    [ForeignKey(nameof(NotificationId))]
    [InverseProperty(nameof(Notification.NotificationAdmins))]
    public virtual Notification LaNotification { get; set; } = null!;
}