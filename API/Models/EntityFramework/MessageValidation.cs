using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_message_validation_mesval")]
public class MessageValidation 
{
    [Key]
    [Column("mesval_id")]
    public int MessageValidationId { get; set; }
    
    //id des relations avec les autres tables : 
    [Column("mesval_message_id")]
    public int MessageId { get; set; }
    
    //relation avec les autres tables : 
    [ForeignKey(nameof(MessageId))]
    [InverseProperty(nameof(Message.MessageValidation))]
    public virtual Message Message{ set; get; }
}