using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_e_notification_com_notcom")]
public class NotificationCommercial : IEntity
{
    [Key]
    [Column("notcom_id")]
    public int NotificationCommercialId { get; set; }

    [Column("notcom_com_text")]
    public String CommercialText { get; set; }
    [Column("notcom_com_title")]
    public String CommercialTitle { get; set; }

    // relation avec la table notification
    [Column("notcom_notification_id")]
    public int NotificationId { get; set; }

    [ForeignKey(nameof(NotificationId))]
    [InverseProperty(nameof(Notification.NotificationCommercials))]
    public virtual Notification LaNotification { get; set; } = null!;

    public int GetId() => NotificationCommercialId;
}