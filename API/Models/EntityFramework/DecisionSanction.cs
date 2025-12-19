using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models.EntityFramework
{
    [Table("t_e_sanction_san")]
    public class DecisionSanction
    {
        [Key]
        [Column("san_id")]
        public int DecisionSanctionId { get; set; }
        
        [Column("san_en_cours")]
        public bool EstEnCours { get; set; }
        
        //Id table Decision : 
        
        [Column("san_decision_id")]
        public int DecisionId { get; set; }
        
        //relation avec la table decision : 
        [ForeignKey(nameof(DecisionId))]
        [InverseProperty(nameof(Decision.DecisionSanction))]
        public virtual Decision Decision { get; set; } = null!;
        
        //relation avec les types de sanctions : 
        
        
        //suspension
        [InverseProperty(nameof(SanctionSuspension.DecisionSanction))]
        public virtual SanctionSuspension? SanctionSuspension { get; set; } 
        
        //Bannissement
        [InverseProperty(nameof(SanctionBannissement.DecisionSanction))]
        public virtual SanctionBannissement? SanctionBannissement { get; set; } 
        
        //relation avec la table Element de decision
        [InverseProperty(nameof(ElementDecision.DecisionSanction))]
        public virtual ElementDecision ElementDecision { get; set; } = null!; 
    }
}
