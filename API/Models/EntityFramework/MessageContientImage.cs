using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_j_message_contient_image_messconima")]
public class MessageContientImage : IEntity
{
    [Key]
    [Column("messconima_id")]
    public int  MessageContientImageId { get; set; }
    
    //id des autres tables : 
    [Column("messconima_message_texte_id")]
    public int MessageTexteId { get; set; }
    
    [Column("messconima_image_id")]
    public int PhotoId { get; set; }
    
    //relation avec les autres tables : 
    
    [ForeignKey(nameof(MessageTexteId))]
    [InverseProperty(nameof(MessageTexte.Photos))]
    public virtual MessageTexte Message{ set; get; }
    
    [ForeignKey(nameof(PhotoId))]
    [InverseProperty(nameof(Photo.Messages))]
    public virtual Photo Photo { set; get; }

    public int GetId()
    {
        return MessageContientImageId;
    }
}