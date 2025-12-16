using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework
{
    [Table("t_e_element_decision_eledec")]
    public class ElementDecision
    {
        [Key]
        [Column("eledec_id")]
        public int ElementDecisionId { get; set; }

        [InverseProperty(nameof(Sanction.Elementdecision))]
        public virtual ICollection<Sanction> Sanctions { get; set; } = new List<Sanction>();

        [InverseProperty(nameof(ElementDecisionAnnonce.Elementdecision))]
        public virtual ICollection<ElementDecisionAnnonce> ElementDecisionAnnonces { get; set; } = new List<ElementDecisionAnnonce>();

        [InverseProperty(nameof(ElementDecisionAvis.Elementdecision))]
        public virtual ICollection<ElementDecisionAvis> Elementdecisionavis { get; set; } = new List<ElementDecisionAvis>();

        [InverseProperty(nameof(ElementDecisionUtilisateur.Elementdecision))]
        public virtual ICollection<ElementDecisionUtilisateur> Elementdecisionutilisateur { get; set; } = new List<ElementDecisionUtilisateur>();

        [InverseProperty(nameof(ElementDecisionMessage.Elementdecision))]
        public virtual ICollection<ElementDecisionMessage> Elementdecisionmessages { get; set; } = new List<ElementDecisionMessage>();

    }
}
