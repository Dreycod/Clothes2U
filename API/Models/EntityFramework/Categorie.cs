using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using API.Attributes;

namespace API.Models.EntityFramework;

[Table("t_e_categorie_cat")]
public class Categorie : IEntityWithNavigation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("cat_id")]
    public int CategorieId { get; set; }
    
    [Column("cat_libelle_categorie")]
    public string LibelleCategorie { get; set; } = null!;
    
    //relations avec les autres tables
    [InverseProperty(nameof(SousCategorie.Categorie))]
    [NavigationProperty]
    public virtual ICollection<SousCategorie> SousCategories { get; set; } = new List<SousCategorie>();
    
    [InverseProperty(nameof(Annonce.Categorie))]
    public virtual ICollection<Annonce> Annonces { get; set; } = new List<Annonce>();
    
    [InverseProperty(nameof(Taille.Categorie))]
    public virtual ICollection<Taille> Tailles { get; set; } = new List<Taille>();

    public int GetId() => CategorieId;

}