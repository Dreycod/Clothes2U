using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_message_demande_mesdem")]
public class MessageDemande : IEntity
{
    [Key]
    [Column("mesdem_id")]
    public int MessageDemandeId { get; set; }
    
    //Id avec les autres tables : 
    [Column("mesdem_message_id")]
    public int MessageId { get; set; }
    
    [Column("mesdem_demande_id")]
    public int? DemandeId { get; set; }
    
    //relation avec les autres tables
    
    [ForeignKey(nameof(DemandeId))]
    [InverseProperty(nameof(MessageDemande.ContreOffres))]
    public virtual MessageDemande? Offre { set; get; }
    
    [ForeignKey(nameof(MessageId))]
    [InverseProperty(nameof(Message.MessageDemande))]
    public virtual Message Message{ set; get; }
    
    [InverseProperty(nameof(MessageDemande.Offre))]
    public virtual ICollection<MessageDemande>? ContreOffres { get; set; } = new List<MessageDemande>();
    
    public int GetId() => MessageDemandeId;
}