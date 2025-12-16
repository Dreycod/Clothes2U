using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models.EntityFramework
{
    [Table("t_e_sanction_san")]
    public class Sanction
    {
        [Key]
        [Column("san_id")]
        public int SanctionId { get; set; }

        [Column("san_statut")]
        public bool Statut { get; set; }

        [Column("san_decision_suspension_id")]
        public int DecisionSuspensionId { get; set; }

        [Column("san_element_decision_id")]
        public int ElementDecisionId { get; set; }

        [ForeignKey(nameof(DecisionSuspensionId))]
        [InverseProperty(nameof(Decision_suspension.Sanctions))]
        public virtual Decision_suspension Decision_sus { get; set; }

        [ForeignKey(nameof(ElementDecisionId))]
        [InverseProperty(nameof(ElementDecision.Sanctions))]
        public virtual ElementDecision Elementdecision { get; set; }

        [InverseProperty(nameof(Suspension.SanctionSus))]
        public virtual ICollection<Suspension> Suspensions { get; set; } = new List<Suspension>();

        [InverseProperty(nameof(Ban.SanctionBan))]
        public virtual ICollection<Ban> Bans { get; set; } = new List<Ban>();

    }
}
