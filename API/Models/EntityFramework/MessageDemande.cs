using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_message_mes")]
public class MessageDemande : Message
{
    [Column("mes_demande_id")]
    public int? DemandeId { get; set; }
    
    [ForeignKey(nameof(DemandeId))]
    [InverseProperty(nameof(MessageDemande.ContreOffres))]
    public virtual MessageDemande? Offre { set; get; }
    
    [InverseProperty(nameof(MessageDemande.Offre))]
    public virtual ICollection<MessageDemande>? ContreOffres { get; set; } = new List<MessageDemande>();
}