using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_signalement_message_sigmes")]
public class SignalementMessage : IEntity
{
    [Key]
    [Column("sigmes_id")]
    public int SignalementMessageId { get; set; }
    
    //relation avec la table signalement
    [Column("sigmes_signalement_id")]
    public int SignalementId { get; set; }
    
    [ForeignKey(nameof(SignalementId))]
    [InverseProperty(nameof(Signalement.SignalementsMessage))]
    public virtual Signalement Signalement { get; set; } = null!;
    
    //relation avec la table message : 
    [Column("sigmes_message_id")]
    public int MessageId { get; set; }
    
    [ForeignKey(nameof(MessageId))]
    [InverseProperty(nameof(Message.Signalements))]
    public virtual Message Message { get; set; } = null!;
    
    
    
    public int GetId() =>  SignalementMessageId;
}