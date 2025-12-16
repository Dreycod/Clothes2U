using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework
{
    [Table("t_e_element_decision_annonce_eleann")]
    public class ElementDecisionAnnonce
    {
        [Key]
        [Column("eleann_id")]
        public int ElementDecisionAnnonceId { get; set; }

        [Column("eleann_element_decision_id")]
        public int ElementDecisionId { get; set; }

        [Column("eleann_annonce_id")]
        public int AnnonceId { get; set; }

        [ForeignKey(nameof(ElementDecisionId))]
        [InverseProperty(nameof(ElementDecision.ElementDecisionAnnonces))]
        public virtual ElementDecision Elementdecision { get; set; }

        [ForeignKey(nameof(AnnonceId))]
        [InverseProperty(nameof(Annonce.ElementDecisionAnnonces))]
        public virtual Annonce AnnonceElmtDecision { get; set; }
    }
}
