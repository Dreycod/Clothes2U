using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_e_couleur_cou")]
public class Couleur
{
    [Key]
    [Column("cou_id")]
    public int CouId { get; set; }
    
    [Column("cou_nom")]
    public string Nom { get; set; } = null!;
    
    //relation avec la table annonce : 
    
    [InverseProperty(nameof(Annonce.Couleur))]
    public virtual ICollection<Annonce> Annonces { get; set; } = new List<Annonce>();
}