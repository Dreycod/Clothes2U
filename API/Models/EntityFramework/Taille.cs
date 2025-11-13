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
    
    //id de la table catégorie
    
    [Column("tai_categorie_taille_id")]
    public int CategorieTailleId { get; set; }
    
    //relations avec les autres tables
    
    [InverseProperty(nameof(Annonce.Taille))]
    public virtual ICollection<Annonce> Annonces { get; set; } = new List<Annonce>();
    
    [ForeignKey(nameof(CategorieTailleId))]
    [InverseProperty(nameof(Categorie.Tailles))]
    public virtual Categorie Categorie { get; set; } = null!;
    
    public int GetId() => TailleId;
}