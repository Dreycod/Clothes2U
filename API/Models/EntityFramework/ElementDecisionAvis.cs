using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework
{
    [Table("t_e_element_decision_avis_eleavs")]
    public class ElementDecisionAvis
    {
        [Key]
        [Column("eleavs_id")]
        public int ElementDecisionAvisId { get; set; }

        [Column("eleavs_element_decision_id")]
        public int ElementDecisionId { get; set; }

        [Column("eleavs_avis_id")]
        public int AvisId { get; set; }

        [ForeignKey(nameof(ElementDecisionId))]
        [InverseProperty(nameof(ElementDecision.Elementdecisionavis))]
        public virtual ElementDecision Elementdecision { get; set; }

        [ForeignKey(nameof(AvisId))]
        [InverseProperty(nameof(NoteUtilisateur.Elementsdecisionavis))]
        public virtual NoteUtilisateur Avis { get; set; }
    }
}
