using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework
{
    [Table("t_e_element_decision_utilisateur_eleuti")]
    public class ElementDecisionUtilisateur
    {
        [Key]
        [Column("eleuti_id")]
        public int ElementDecisionUtilisateurId { get; set; }

        [Column("eleuti_element_decision_id")]
        public int ElementDecisionId { get; set; }

        [Column("eleuti_utilisateur_id")]
        public int UtilisateurId { get; set; }

        [ForeignKey(nameof(ElementDecisionId))]
        [InverseProperty(nameof(ElementDecision.Elementdecisionutilisateur))]
        public virtual ElementDecision Elementdecision { get; set; }

        [ForeignKey(nameof(UtilisateurId))]
        [InverseProperty(nameof(Utilisateur.ElementDecisionUtilisateurs))]
        public virtual Utilisateur UtilisateurElmtDecisionUti { get; set; }
    }
}
