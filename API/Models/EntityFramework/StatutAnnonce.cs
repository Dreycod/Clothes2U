using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_statut_annonce_staann")]
public class StatutAnnonce
{
    [Key]
    [Column("staann_id")]
    public int StatutAnnonceId { get; set; }
    
    [Column("staan_libelle_statut")]
    public string StatutLibelle { get; set; } = null!;
    
    //relation avec les autres tables
    
    [InverseProperty(nameof(Annonce.Statut))]
    public virtual ICollection<Annonce> Annonces { get; set; } = new List<Annonce>();
}