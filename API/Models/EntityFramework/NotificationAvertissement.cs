using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_notification_not")]
public class NotificationAvertissement : Notification
{
    [Column("not_avertissement_message")]
    public string MessageAvertissement  { get; set; } = null!;
    
    
}