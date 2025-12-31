using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_message_validation_mesval")]
public class MessageValidation : IEntity
{
    [Key]
    [Column("mesval_id")]
    public int MessageValidationId { get; set; }
    
    //id des relations avec les autres tables : 
    [Column("mesval_message_id")]
    public int MessageId { get; set; }
    
    [Column("mesval_proposition_validee_id")]
    public int MessageDemandeId  { get; set; }
    
    [Column("mesval_est_acceptee")]
    public bool EstAcceptee { get; set; }
    
    //relation avec les autres tables : 
    [ForeignKey(nameof(MessageId))]
    [InverseProperty(nameof(Message.MessageValidation))]
    public virtual Message Message{ set; get; }
    
    [ForeignKey(nameof(MessageDemandeId))]
    [InverseProperty(nameof(MessageDemande.Validation))]
    public virtual MessageDemande PropositionValidee { set; get; }


    public int GetId() => MessageValidationId;
}