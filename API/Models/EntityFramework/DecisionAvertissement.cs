using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework
{
    [Table("t_e_decision_avertissement_decave")]
    public class DecisionAvertissement : IEntity
    {
        [Key]
        [Column("decave_id")]
        public int DecisionAvertissementId { get; set; }
        
        //id avec la table decision
        [Column("decave_decision_id")]
        public int DecisionId { get; set; }
        
        //relation avec la table decision : 
        [ForeignKey(nameof(DecisionId))]
        [InverseProperty(nameof(Decision.DecisionAvertissement))]
        public virtual Decision Decision { get; set; } = null!;

        public int GetId() => DecisionAvertissementId;
    }
}
