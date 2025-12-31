using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_notification_proposition_notpro")]
public class NotificationProposition : IEntity
{
    [Key]
    [Column("notpro_id")]
    public int NotificationPropositionId { get; set; }
    
    [Column("notpro_proposition_id")]
    public int PropositionId { get; set; }
    
    [Column("notpro_notification_id")]
    public int NotificationId { get; set; }

    [InverseProperty(nameof(MessageDemande.NotificationProposition))]
    public virtual MessageDemande MessageDemande{ set; get; }
    
    [ForeignKey(nameof(NotificationId))]
    [InverseProperty(nameof(Notification.NotificationProposition))]
    public virtual Notification Notification { get; set; } = null!;
    

    public int GetId() =>  NotificationPropositionId;
}