using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_message_texte_mestex")]
public class MessageTexte : IEntity
{
    [Key]
    [Column("mestex_id")]
    public int MessageTexteId { get; set; }
    
    [Column("mestex_contenu_message")]
    public string Content { get; set; }
    
    //id de relation avec les autres tables : 
    
    [Column("mestex_message_id")]
    public int MessageId { get; set; }
    
    //relation avec les autres tables : 
    [ForeignKey(nameof(MessageId))]
    [InverseProperty(nameof(Message.MessageTexte))]
    public virtual Message Message{ set; get; }
    
    [InverseProperty(nameof(MessageContientImage.Message))]
    public virtual ICollection<MessageContientImage>? Photos { get; set; } 
    
    public int GetId() => MessageTexteId;
}