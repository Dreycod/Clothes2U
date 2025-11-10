using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_adresse_adr")]
public class Adresse
{
    [Key]
    [Column("adr_id")]
    public int AdresseId { get; set; }
    
    [Column("adr_rue")]
    [MaxLength(80)]
    public string AdresseRue { get; set; }
    
    [Column("adr_ville")]
    [MaxLength(80)]
    public string AdresseVille { get; set; }
    
    [Column("adr_code_postal")]
    [RegularExpression(@"^\d{6}$", ErrorMessage = "Le code postal doit contenir exactement 5 chiffres.")]
    public string AdrCodePostal { get; set; }
    
    //relation avec la table utilisateur
    
    [InverseProperty(nameof(Utilisateur.Adresse))]
    public virtual ICollection<Utilisateur> Utilisateurs { get; set; } = new List<Utilisateur>();
}