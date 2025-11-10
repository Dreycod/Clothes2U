using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_marque_mar")]
public class Marque
{
    [Key]
    [Column("mar_id")]
    public int MarqueId { get; set; }
    
    [Column("mar_nommarque")]
    public string NomMarque { get; set; } = null!;
    
    //relation avec la table utilisateur
    
    [InverseProperty(nameof(Annonce.Marque))]
    public virtual ICollection<Annonce> Annonces { get; set; } = new List<Annonce>();
}