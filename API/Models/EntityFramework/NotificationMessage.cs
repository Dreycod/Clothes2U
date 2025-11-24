using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_notification_message_notmes")]
public class NotificationMessage
{
    [Key]
    [Column("notmes_id")]
    public int NotificationMessageId { get; set; }
    [Column("notmes_message_id")]
    public int MessageId { get; set; }
    
    [Column("notmes_notification_id")]
    public int NotificationId { get; set; }
    //relation avec les autres tables :
    
    [ForeignKey(nameof(MessageId))]
    [InverseProperty(nameof(Message.NotificationsMessage))]
    public virtual Message Message { get; set; } = null!;
    
    [ForeignKey(nameof(NotificationId))]
    [InverseProperty(nameof(Notification.NotificationMessages))]
    public virtual Notification LaNotification { get; set; } = null!;

}