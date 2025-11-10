using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace API.Models.EntityFramework;


[Table("t_e_conversation_con")]
public class Conversation
{
    [Key]
    [Column("con_id")]
    public int ConversationId { get; set; }
    
    [Column("con_datecreation")]
    public DateTime CreationDate { get; set; }
    
    //relation avec les autres tables
    [InverseProperty(nameof(Message.Conversation))]
    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}