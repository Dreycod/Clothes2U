using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_j_illustre_annonce_illann")]
public class Illustre_Annonce
{
    [Key]
    [Column("illann_id")]
    public int IllustId { get; set; }
    
    //id des relations
    [Column("illann_annonce_id")]
    public int AnnonceId { get; set; }
    
    [Column("illann_photo_id")]
    public int PhotoId { get; set; }
    
    //relation avec les autres tables
    
    [ForeignKey(nameof(AnnonceId))]
    [InverseProperty(nameof(Annonce.Photos))]
    public virtual Annonce Annonce { get; set; } = null!;
    
    [ForeignKey(nameof(PhotoId))]
    [InverseProperty(nameof(Photo.Annonces))]
    public virtual Photo Photo { get; set; } = null!;
}