using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_notification_not")]
public class NotificationModificationAnnonce : Notification
{
    [Column("not_annonce_id")]
    public int AnnonceId { get; set; }
    
    //relations avec d'autres tables : 
    
    [ForeignKey(nameof(AnnonceId))]
    [InverseProperty(nameof(Annonce.NotificationsModificationAnnonces))]
    public virtual Annonce Annonce { get; set; } = null!;
}