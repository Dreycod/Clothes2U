using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Enums;

namespace API.Models.EntityFramework;

[Table("t_e_transaction_tra")]
public class Transaction : IEntity
{
    [Key]
    [Column("tra_id")]
    public int TransactionId { get; set; }
    
    [Column("tra_montant")]
    public int TransactionMontant { get; set; }
    
    [Column("tra_transaction_etat")]
    public TransactionEtatEnum TransactionEtat { get; set; }
    
    //id des relations
    [Column("tra_conversation_id")]
    public int ConversationId { get; set; }
    
    //relations avec les autres tables
    [ForeignKey(nameof(ConversationId))]
    [InverseProperty(nameof(Conversation.Transaction))]
    public virtual Conversation Conversation { get; set; } = null!;

    public int GetId() => TransactionId;

}