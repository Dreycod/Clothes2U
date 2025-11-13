using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_notification_not")]
public class NotificationMessage : Notification
{
    [Column("not_message_id")]
    public int MessageId { get; set; }
    
    //relation avec les autres tables :
    
    [ForeignKey(nameof(MessageId))]
    [InverseProperty(nameof(Message.NotificationsMessage))]
    public virtual Message Message { get; set; } = null!;
}