using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_e_element_decision_avis_edav")]
public class ElementDecisionAvis : IEntity
{
    [Key]
    [Column("edav_id")]
    public int ElementDecisionAvisId { get; set; }
    
    //avis
    
    [Column("edav_annonce_id")]
    public int AvisId { get; set; }
    
    [ForeignKey(nameof(AvisId))]
    [InverseProperty(nameof(Avis.Decisions))]
    public virtual NoteUtilisateur Avis { get; set; } = null!;
    
    //element decision
    
    [Column("edav_element_decision_id")]
    public int ElementDecisionId { get; set; }
    
    [ForeignKey(nameof(ElementDecisionId))]
    [InverseProperty(nameof(ElementDecision.ElementDecisionAvis))]
    public virtual ElementDecision ElementDecision { get; set; } = null!;
    
    public int GetId() => ElementDecisionAvisId;
}