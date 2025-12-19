using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;



[Table("t_e_sanction_bannissement_sanban")]
public class SanctionBannissement : IEntity
{
    [Key]
    [Column("sanban_id")]
    public int SanctionBannissementId { get; set; }
    
    //id de la table decisionSanction
    [Column("sanban_decision_sanction_id")]
    public int DecisionSanctionId { get; set; }
    
    //relation avec la table decision sanction
    [ForeignKey(nameof(DecisionSanctionId))]
    [InverseProperty(nameof(DecisionSanction.SanctionBannissement))]
    public virtual DecisionSanction DecisionSanction { get; set; } = null!;
    
    public int GetId() => SanctionBannissementId;
}