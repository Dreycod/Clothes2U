using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_e_element_decision_message_edm")]
public class ElementDecisionMessage : IEntity
{
    [Key]
    [Column("edm_id")]
    public int ElementDecisionMessageId { get; set; }
    
    [Column("edm_message_id")]
    public int MessageId { get; set; }
    
    [ForeignKey(nameof(MessageId))]
    [InverseProperty(nameof(Message.Decisions))]
    public virtual Message Message { get; set; } = null!;
    
    [Column("edm_element_decision_id")]
    public int ElementDecisionId { get; set; }
    
    [ForeignKey(nameof(ElementDecisionId))]
    [InverseProperty(nameof(ElementDecision.ElementDecisionMessage))]
    public virtual ElementDecision ElementDecision { get; set; } = null!;
    
    public int GetId() => ElementDecisionMessageId;
    
}