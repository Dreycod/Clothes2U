using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_notification_not")]
public class NotificationNouvelleAnnonce : Notification
{
    [Column("not_annonce_id")]
    public int AnnonceId { get; set; }
    
    //relation avec les autres tables : 
    
    [ForeignKey(nameof(Annonce))]
    [InverseProperty(nameof(Annonce.NotificationsNouvelleAnnonces))]
    public virtual Annonce Annonce { get; set; } = null!;
}