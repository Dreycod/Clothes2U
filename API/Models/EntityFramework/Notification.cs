using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_notification_not")]
public abstract class Notification
{
    [Key]
    [Column("not_id")]
    public int NotificationId { get; set; }
    
    //id des relations : 
    
    [Column("not_type_id")]
    public int NotificationTypeId { get; set; }
    
    [Column("not_utilisateur_id")]
    public int UtilisateurId { get; set; }
    
    //relation avec les autres tables
    
    [ForeignKey(nameof(NotificationId))]
    [InverseProperty(nameof(NotificationType.Notifications))]
    public virtual NotificationType NotificationType { get; set; } = null!;
    
    [ForeignKey(nameof(UtilisateurId))]
    [InverseProperty(nameof(Utilisateur.Notifications))]
    public virtual Utilisateur Utilisateur { get; set; } = null!;
}