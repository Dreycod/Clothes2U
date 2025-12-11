using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_notification_avertissement_notave")]
public class NotificationAvertissement : IEntity
{
    [Key]
    [Column("notave_id")]
    public int NotificationAvertissementId { get; set; }
    [Column("notave_avertissement_message")]
    public string MessageAvertissement  { get; set; } = null!;
    
    [Column("notave_notification_id")]
    public int NotificationId { get; set; }

    [ForeignKey(nameof(NotificationId))]
    [InverseProperty(nameof(Notification.NotificationAvertissements))]
    public virtual Notification LaNotification { get; set; } = null!;
    
    public int GetId() => NotificationAvertissementId;
    
}