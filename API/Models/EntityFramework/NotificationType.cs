using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_notification_type_nottyp")]
public class NotificationType
{
    [Key]
    [Column("nottyp_id")]
    public int NotificationTypeId { get; set; }
    
    [Column("nottyp_libelle_type")]
    public string LibelleType { get; set; }
    
    //relation avec les autres tables : 
    
    [InverseProperty(nameof(Message.Utilisateur))]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
}