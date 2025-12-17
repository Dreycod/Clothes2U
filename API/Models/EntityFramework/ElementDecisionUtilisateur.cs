using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_e_element_decision_utilisateur_edu")]
public class ElementDecisionUtilisateur : IEntity
{
    [Key]
    [Column("edu_id")]
    public int ElementDecisionUtilisateurId { get; set; }
    
    //relation avec la table element decision
    [Column("edu_element_decision_id")]
    public int ElementDecisionId { get; set; }
    
    [ForeignKey(nameof(ElementDecisionId))]
    [InverseProperty(nameof(ElementDecision.ElementDecisionUtilisateur))]
    public virtual ElementDecision ElementDecision { get; set; } = null!;
    
    public int GetId() => ElementDecisionUtilisateurId;
}