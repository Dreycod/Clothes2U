using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_e_element_decision_eledec")]
public class ElementDecision : IEntity
{
    [Key]
    [Column("eledec_id")]
    public int ElementDecisionId { get; set; }
    
    [Column("eledec_sanction_id")]
    public int DecisionSanctionId { get; set; }
    
    //relation avec la table DecisionSanction
    [ForeignKey(nameof(DecisionSanctionId))]
    [InverseProperty(nameof(DecisionSanction.ElementDecision))]
    public virtual DecisionSanction DecisionSanction { get; set; } = null!;
    
    
    //relation avec les autres elements de décision : 
    [InverseProperty(nameof(ElementDecisionAnnonce.ElementDecision))]
    public virtual ElementDecisionAnnonce? ElementDecisionAnnonce { get; set; } = null!;
    
    [InverseProperty(nameof(ElementDecisionAvis.ElementDecision))]
    public virtual ElementDecisionAvis? ElementDecisionAvis { get; set; } = null!;
    
    [InverseProperty(nameof(ElementDecisionMessage.ElementDecision))]
    public virtual ElementDecisionMessage? ElementDecisionMessage { get; set; } = null!;
    
    [InverseProperty(nameof(ElementDecisionUtilisateur.ElementDecision))]
    public virtual ElementDecisionUtilisateur? ElementDecisionUtilisateur { get; set; } = null!;
    
    public int GetId() => ElementDecisionId;
}