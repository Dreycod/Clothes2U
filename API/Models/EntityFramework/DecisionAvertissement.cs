using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework
{
    [Table("t_e_decision_avertissement_decave")]
    public class DecisionAvertissement
    {
        [Key]
        [Column("decave_id")]
        public int DecisionAvertissementId { get; set; }

        [Column("decave_date_avertissement")]
        public DateTime DateAvertissement { get; set; }

        [Column("decave_motif_avertissement")]
        public string? MotifAvertissement { get; set; }

        [Column("decave_decision_suspension_id")]
        public int DecisionSuspensionId { get; set; }

        [ForeignKey(nameof(DecisionSuspensionId))]
        [InverseProperty(nameof(Decision_suspension.DecisionAvertissements))]
        public virtual Decision_suspension DecisionSuspension { get; set; }
    }
}
