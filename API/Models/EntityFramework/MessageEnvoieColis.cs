using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_message_envoie_colis_mesenvcol")]
public class MessageEnvoieColis : IEntity
{
    [Key]
    [Column("mesenvcol_id")]
    public int MessageEnvoieColisId { get; set; }
    
    //id des autres tables :
    
    [Column("mesenvcol_message_id")]
    public int MessageId { get; set; }
    
    [Column("mesenvcol_photo_id")]
    public int PhotoId { get; set; }
    
    [Column("mesenvcol_est_payee_id")]
    public int MessageEstPayeeId { get; set; }

    [ForeignKey(nameof(MessageId))]
    [InverseProperty(nameof(Message.MessageEnvoieColis))]
    public virtual Message Message { get; set; }
    
    [ForeignKey(nameof(PhotoId))]
    [InverseProperty(nameof(Photo.MessageEnvoieColis))]
    public virtual Photo PhotoPreuve { get; set; }

    [ForeignKey(nameof(MessageEstPayeeId))]
    [InverseProperty(nameof(MessageEstPayee.MessageEnvoieColis))]
    public virtual MessageEstPayee MessageEstPayee { get; set; }

    public int GetId() => MessageEnvoieColisId;
}