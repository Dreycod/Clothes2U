using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_e_photo_pho")]
public class Photo : IEntity
{
    [Key]
    [Column("pho_photo_id")]
    public int PhotoId { get; set; }
    
    [Column("pho_image", TypeName = "bytea")]
    public byte[] Image { get; set; }
    
    //relaiton avec les autres tables
    
    [InverseProperty(nameof(Illustre_Annonce.Photo))]
    public virtual ICollection<Illustre_Annonce> Annonces { get; set; } = new List<Illustre_Annonce>();
    
    
    [InverseProperty(nameof(MessageContientImage.Photo))]
    public virtual ICollection<MessageContientImage>? Messages { get; set; } = new List<MessageContientImage>();
    
    public int GetId() => PhotoId;
}