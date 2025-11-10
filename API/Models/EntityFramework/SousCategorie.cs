using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_e_souscategorie_sscat")]
public class SousCategorie
{
    [Key]
    [Column("sscat_id")]
    public int SousCategorieId { get; set; }
    
    [Column("sscat_libelle_sous_categorie")]
    public string LibelleSousCategorie { get; set; } = null!;
    
    //id de la table categorie
    
    [Column("sscat_categorie_id")]
    public int CategorieId { get; set; }
    
    //relation avec les autres tables
    
    [ForeignKey(nameof(CategorieId))]
    [InverseProperty(nameof(Categorie.SousCategories))]
    public virtual Categorie Categorie { get; set; } = null!;
    
    [InverseProperty(nameof(Annonce.SousCategorie))]
    public virtual ICollection<Annonce> Annonces { get; set; } = new List<Annonce>();
}