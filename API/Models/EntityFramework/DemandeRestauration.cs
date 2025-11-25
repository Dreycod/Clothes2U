using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework
{
    [Table("t_e_demande_restauration_demres")]
    public class DemandeRestauration : IEntity
    {
        [Key]
        [Column("demres_id")]
        public int DemandeRestaurationId { get; set; }

        [Column("demres_utilisateur_id")]
        public int UtilisateurId { get; set; }

        [Column("demres_suspension_id")]
        public int SuspensionId { get; set; }

        [Column("demres_demande_restauration")]
        public string? DemandeRestaurationText { get; set; }

        [ForeignKey(nameof(UtilisateurId))]
        [InverseProperty(nameof(Utilisateur.DemandesRestauration))]
        public Utilisateur Plaignant { get; set; } = null!;

        [ForeignKey(nameof(SuspensionId))]
        [InverseProperty(nameof(Decision_suspension.DemandesRes))]
        public Decision_suspension Suspension { get; set; } = null!;
        public int GetId() => DemandeRestaurationId;

    }
}
