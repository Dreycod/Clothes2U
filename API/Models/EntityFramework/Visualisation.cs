using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework
{
    [Table("t_j_visualisation_vis")]
    public class Visualisation : IEntity
    {
        [Key]
        [Column("vis_id")]
        public int VisualisationId { get; set; }

        [Column("vis_utilisateur_id")]
        public int UtilisateurId { get; set; }

        [Column("vis_annonce_id")]
        public int AnnonceId { get; set; }

        [Column("vis_date")]
        public DateTime DateVisualisation { get; set; } = DateTime.UtcNow;

        // Relations
        [ForeignKey(nameof(UtilisateurId))]
        [InverseProperty(nameof(Utilisateur.Visualisations))]
        public virtual Utilisateur UtilisateurVisu { get; set; } = null!;

        [ForeignKey(nameof(AnnonceId))]
        [InverseProperty(nameof(Annonce.LesVisualisations))]

        public virtual Annonce AnnonceVisu { get; set; } = null!;

        public int GetId() => VisualisationId;
    }
}
