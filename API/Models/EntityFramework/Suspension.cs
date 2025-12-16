using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Models.EntityFramework
{
    [Table("t_e_suspension_susp")]
    public class Suspension
    {
        [Key]
        [Column("susp_id")]
        public int SuspensionId { get; set; }

        [Column("susp_date_fin_suspension")]
        public DateTime DateFinSuspension { get; set; }

        [Column("susp_sanction_id")]
        public int SanctionId { get; set; }

        [ForeignKey(nameof(SanctionId))]
        [InverseProperty(nameof(Sanction.Suspensions))]
        public virtual Sanction SanctionSus { get; set; }
    }
}
