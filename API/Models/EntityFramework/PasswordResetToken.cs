using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework
{
    [Table("t_e_password_reset")]
    public class PasswordResetToken : IEntity
    {
        [Key]
        [Column("reset_id")]
        public int PasswordResetTokenId { get; set; }

        [Column("reset_token")]
        public string Token { get; set; }

        [Column("reset_expiration")]
        public DateTime Expiration { get; set; }

        [Column("reset_used")]
        public bool Used { get; set; }

        [Column("reset_utilisateur_id")]
        public int UtilisateurId { get; set; }

        [ForeignKey(nameof(UtilisateurId))]
        [InverseProperty(nameof(Utilisateur.PasswordResetTokens))]
        public virtual Utilisateur UtilisateurReset { get; set; }

        public int GetId() => PasswordResetTokenId;
    }
}
