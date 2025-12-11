using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Attributes;

namespace API.Models.EntityFramework
{
    [Table("t_j_mesure_mes")]
    public class Mesure
    {
        [Key]
        [Column("mes_id")]
        public int MesureId { get; set; }

        [Column("mes_taille_id")]
        public int TailleId { get; set; }

        [Column("mes_categorie_id")]
        public int CategorieId { get; set; }

        [ForeignKey(nameof(TailleId))]
        [InverseProperty(nameof(Taille.Mesures))]
        [NavigationProperty]
        public virtual Taille TailleMesure { get; set; } = null!;

        [ForeignKey(nameof(CategorieId))]
        [InverseProperty(nameof(Categorie.Mesures))]
        [NavigationProperty]
        public virtual Categorie CategorieMesure { get; set; } = null!;
    }
}
