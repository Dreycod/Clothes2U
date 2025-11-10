using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_message_mes")]
public abstract class Message
{
    [Key]
    [Column("mes_id")]
    public int MessageId { get; set; }
    
    [Column("mes_date_envoie")]
    public DateTime MessageDate { get; set; }
    
    [Column("mes_lu")]
    public bool MessageLu { get; set; }
    
    //relation avec id
    [Column("mes_utilisateur_id")]
    public int UtilisateurId { get; set; }
    
    [Column("mes_conversation_id")]
    public int  ConversationId { get; set; }
    
    //relation avec les autres tables
    
    [ForeignKey(nameof(UtilisateurId))]
    [InverseProperty(nameof(Utilisateur.Messages))]
    public virtual Utilisateur Utilisateur { get; set; } = null!;
    
    [ForeignKey(nameof(ConversationId))]
    [InverseProperty(nameof(Conversation.Messages))]
    public virtual Conversation Conversation { get; set; } = null!;
}