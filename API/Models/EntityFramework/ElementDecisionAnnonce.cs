using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_e_element_decision_annonce_eda")]
public class ElementDecisionAnnonce : IEntity
{
    [Key]
    [Column("eda_id")]
    public int ElementDecisionAnnonceId { get; set; }
    
    [Column("eda_annonce_id")]
    public int AnnonceId { get; set; }
    
    [ForeignKey(nameof(ElementDecisionAnnonceId))]
    [InverseProperty(nameof(Annonce.Decisions))]
    public virtual Annonce Annonce { get; set; } = null!;
    
    [Column("eda_element_decision_id")]
    public int ElementDecisionId { get; set; }
    
    [ForeignKey(nameof(ElementDecisionAnnonceId))]
    [InverseProperty(nameof(ElementDecision.ElementDecisionAnnonce))]
    public virtual ElementDecision ElementDecision { get; set; } = null!;
    
    public int GetId() => ElementDecisionAnnonceId;
}