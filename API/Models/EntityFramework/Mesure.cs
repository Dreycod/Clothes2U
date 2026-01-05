using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Attributes;

namespace API.Models.EntityFramework
{
    [Table("t_j_mesure_mes")]
    public class Mesure : IEntity
    {
        [Key]
        [Column("mes_id")]
        public int MesureId { get; set; }

        [Column("mes_taille_id")]
        public int TailleId { get; set; }

        [Column("mes_sous_categorie_id")]
        public int SousCategorieId { get; set; }

        [ForeignKey(nameof(TailleId))]
        [InverseProperty(nameof(Taille.Mesures))]
        [NavigationProperty]
        public virtual Taille TailleMesure { get; set; } = null!;

        [ForeignKey(nameof(SousCategorieId))]
        [InverseProperty(nameof(SousCategorie.Mesures))]
        [NavigationProperty]
        public virtual SousCategorie SousCategorieMesure { get; set; } = null!;

        public int GetId() => MesureId;

    }
}
