using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models.EntityFramework
{
    [Table("t_e_ban_ban")]
    public class Ban
    {
        [Key]
        [Column("ban_id")]
        public int BanId { get; set; }

        [Column("ban_sanction_id")]
        public int SanctionId { get; set; }

        [ForeignKey(nameof(SanctionId))]
        [InverseProperty(nameof(Sanction.Bans))]
        public virtual Sanction SanctionBan { get; set; }
    }
}
