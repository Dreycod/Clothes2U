using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_tag_tag")]
public class Tag
{
    [Key]
    [Column("tag_id")]
    public int TagId { get; set; }
    
    [Column("tag_libelle")]
    public string LibelleTag { get; set; }
    
    //relation avec les autres tables : 
    
    [InverseProperty(nameof(Recense.Tag))]
    public virtual ICollection<Recense> Annonces { get; set; } = new List<Recense>();
}