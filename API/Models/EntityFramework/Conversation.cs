using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;


namespace API.Models.EntityFramework;


[Table("t_e_conversation_con")]
public class Conversation
{
    [Key]
    [Column("con_id")]
    public int ConversationId { get; set; }
    
    [Column("con_datecreation")]
    public DateTime CreationDate { get; set; }
    
    //id des relations
    
    [Column("con_annonce_id")]
    public int AnnonceId { get; set; }
    
    [Column("con_statut_conversation_id")]
    public int StatutConversationId { get; set; }
    
    
    //relation avec les autres tables
    [InverseProperty(nameof(Message.Conversation))]
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
    
    [ForeignKey(nameof(StatutConversationId))]
    [InverseProperty(nameof(StatutConversation.Conversations))]
    public virtual StatutConversation? StatutConversation { get; set; }
    
    [InverseProperty(nameof(Transaction.Conversation))]
    public virtual Transaction? Transaction { get; set; }
    
    [InverseProperty(nameof(Achete.Conversation))]
    public virtual Achete? Acheteur { get; set; }
    
    [InverseProperty(nameof(Vend.LaConversation))]
    public virtual Vend? Vendeur { get; set; }
    
    [ForeignKey(nameof(AnnonceId))]
    [InverseProperty(nameof(Annonce.LesConversations))]
    public virtual Annonce LAnnonce {get; set;}
}