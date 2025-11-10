using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_e_photo_pho")]
public class Photo
{
    [Key]
    [Column("pho_photo_id")]
    public int PhotoId { get; set; }
    
    [Column("pho_uri_photo")]
    public string PhotoUri { get; set; }
    
    //relaiton avec les autres tables
    
    [InverseProperty(nameof(Illustre_Annonce.Photo))]
    public virtual ICollection<Illustre_Annonce> Annonces { get; set; } = new List<Illustre_Annonce>();

    [ForeignKey(nameof(MessageTexte.MessageId))]
    [InverseProperty(nameof(MessageTexte.Photos))]
    public virtual MessageTexte? MessageTexte { get; set; }
}