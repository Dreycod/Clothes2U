using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shared.Enums;

namespace API.Models.EntityFramework
{
    [Table("t_e_support_ticket_sup")]
    public class SupportTicket : IEntity
    {
        [Key]
        [Column("sup_id")]
        public int SupportTicketId { get; set; }

        [Column("sup_utilisateur_id")]
        public int UtilisateurId { get; set; }

        [Column("sup_admin_id")]
        public int? AdminId { get; set; }

        [Column("sup_subject")]
        public string Subject { get; set; }

        [Column("sup_message_user")]
        public string MessageUtilisateur { get; set; }

        [Column("sup_message_admin")]
        public string? MessageAdmin { get; set; }

        [Column("sup_created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("sup_answered_at")]
        public DateTime? AnsweredAt { get; set; }

        [Column("sup_status")]
        public StatutTicketEnum Status { get; set; } // OPEN | ANSWERED | CLOSED

        [ForeignKey(nameof(UtilisateurId))]
        public Utilisateur Utilisateur { get; set; }

        [ForeignKey(nameof(AdminId))]
        public Utilisateur? Admin { get; set; }

        public int GetId() => SupportTicketId;
    }
}
