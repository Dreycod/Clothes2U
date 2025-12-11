using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_taille_tai")]
public class Taille : IEntity
{
    [Key]
    [Column("tai_id")]
    public int TailleId { get; set; }
    
    [Column("tai_libelletaille")]
    public string Libelletaille { get; set; }
    
    
    //relations avec les autres tables
    
    [InverseProperty(nameof(Annonce.Taille))]
    public virtual ICollection<Annonce> Annonces { get; set; } = new List<Annonce>();
    

    [InverseProperty(nameof(Mesure.TailleMesure))]
    public virtual ICollection<Mesure> Mesures { get; set; } = new List<Mesure>();
    
    public int GetId() => TailleId;
}