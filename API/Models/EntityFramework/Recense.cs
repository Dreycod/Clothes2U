using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_j_recense_rec")]
public class Recense
{
    [Key]
    [Column("rec_id")]
    public int RecenseId { get; set; }
    
    //id des autres tables :
    [Column("rec_annonce_id")]
    public int AnnonceId { get; set; }
    
    [Column("rec_tag_id")]
    public int TagId { get; set; }
    
    //relation avec les autres tables : 
    
    [ForeignKey(nameof(AnnonceId))]
    [InverseProperty(nameof(Annonce.Tags))]
    public virtual Annonce Annonce { get; set; } = null!;
    
    [ForeignKey(nameof(TagId))]
    [InverseProperty(nameof(Tag.Annonces))]
    public virtual Tag Tag { get; set; } = null!;
}