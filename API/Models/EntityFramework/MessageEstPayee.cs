using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_message_payee_mespay")]
public class MessageEstPayee : IEntity
{
    [Key]
    [Column("mespay_id")]
    public int MessageEstPayeeId { get; set; }
    
    //id des relations avec les autres tables : 
    [Column("mespay_message_id")]
    public int MessageId { get; set; }
    
    [Column("mespay_commande_id")]
    public int CommandeId { get; set; }
    
    [Column("mespay_est_acceptee")]
    public bool EstEnvoye { get; set; } = false;
    
    [Column("mespay_est_annulee")]
    public bool EstAnnule { get; set; } = false;
    
    //relation avec les autres tables : 
    [ForeignKey(nameof(MessageId))]
    [InverseProperty(nameof(Message.MessageEstPayee))]
    public virtual Message Message{ set; get; }
    
    [ForeignKey(nameof(CommandeId))]
    [InverseProperty(nameof(Commande.MessageEstPayee))]
    public virtual Commande Commande { get; set; }
    
    [InverseProperty(nameof(MessageEnvoieColis.MessageEstPayee))]
    public virtual MessageEnvoieColis? MessageEnvoieColis { get; set; }

    public int GetId() => MessageEstPayeeId;
}