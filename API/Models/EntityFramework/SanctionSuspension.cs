using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_e_sanction_suspension_sansus")]
public class SanctionSuspension : IEntity
{
    [Key]
    [Column("sansus_id")]
    public int SanctionSuspensionId { get; set; }
    
    [Column("sansus_date_fin")]
    public DateTime DateFinSuspension { get; set; }
    
    //id de la table decisionSanction
    [Column("sansus_decision_sanction_id")]
    public int DecisionSanctionId { get; set; }
    
    //relation avec la table decision sanction
    [ForeignKey(nameof(DecisionSanctionId))]
    [InverseProperty(nameof(DecisionSanction.SanctionSuspension))]
    public virtual DecisionSanction DecisionSanction { get; set; } = null!;
    public int GetId() => SanctionSuspensionId;
}