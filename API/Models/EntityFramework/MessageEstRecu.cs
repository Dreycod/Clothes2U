using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table( "t_e_message_recu_mesrecu" )]
public class MessageEstRecu : IEntity
{
    [Key]
    [Column("mesrecu_id")]
    public int MessageEstRecuId { get; set; }
    
    [Column("mesrecu_est_conforme")]
    public bool EstConforme { get; set; }
    
    [Column("mesrecu_photo_id")]
    public int? PhotoId { get; set; }
    
    [Column("mesrecu_description")]
    public string? Description { get; set; }
    
    [Column("mesrecu_message_id")]
    public int MessageId { get; set; }
    
    [Column("mesrecu_est_envoie_id")]
    public int MessageEstEnvoieId { get; set; }
    
    [ForeignKey(nameof(MessageId))]
    [InverseProperty(nameof(Message.MessageEstRecu))]
    public virtual Message Message { get; set; }
    
    [ForeignKey(nameof(MessageEstEnvoieId))]
    [InverseProperty(nameof(MessageEnvoieColis.MessageEstRecu))]
    public virtual MessageEnvoieColis MessageEstEnvoie { get; set; }
    
    [ForeignKey(nameof(PhotoId))]
    [InverseProperty(nameof(Photo.MessageEstRecu))]
    public virtual Photo? Photo { get; set; }
    
    public int GetId() => MessageEstRecuId;
}