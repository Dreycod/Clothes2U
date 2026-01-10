using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework
{
    [Table("t_h_utilisateur_transactions_histuti")]
    public class HistoriqueUtilisateur : IEntity
    {
        [Key]
        [Column("histuti_id")]
        public int HistoriqueUtilisateurId { get; set; }
        [Column("histuti_utilisateur_id")]
        public int UtilisateurId { get; set; }

        [Column("histuti_type_transaction")]
        public string? TypeTransaction { get; set; }

        [Column("histuti_annonce_id")]
        public int? AnnonceId { get; set; }

        [Column("histuti_montant")]
        public decimal Montant { get; set; }

        [Column("histuti_date_transaction")]
        public DateTime DateTransaction { get; set; }

        [Column("histuti_deleted_at")]
        public DateTime DateSuppressionCompte { get; set; }

        [Column("histuti_deleted_by_admin_id")]
        public int? AdminId { get; set; }

        [ForeignKey(nameof(UtilisateurId))]
        [InverseProperty(nameof(Utilisateur.HistUtilisateurs))]
        public virtual Utilisateur UserHist { get; set; }

        [ForeignKey(nameof(AdminId))]
        [InverseProperty(nameof(Utilisateur.HistoriquesAdmin))]
        public virtual Utilisateur? Admin { get; set; }

        public int GetId() => HistoriqueUtilisateurId;

    }
}
