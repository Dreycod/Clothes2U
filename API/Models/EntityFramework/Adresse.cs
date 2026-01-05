using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_adresse_adr")]
public class Adresse : IEntity
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
    public string AdresseCodePostal { get; set; }
    
    [Column("adr_pays")]
    public string AdressePays { get; set; } = "France";
    
    [Column("adr_is_default")]
    public bool IsDefault { get; set; }
    
    [Column("adr_utilisateur_id")]
    public int UtilisateurId { get; set; }
    
    //relation avec la table utilisateur
    
    [ForeignKey(nameof(UtilisateurId))]
    [InverseProperty(nameof(Utilisateur.Adresses))]
    public virtual Utilisateur Utilisateurs { get; set; } = null!;
    
    [InverseProperty(nameof(Commande.AdresseLivraison))]
    public virtual ICollection<Commande> Commandes { get; set; } = new List<Commande>();
    
    public int GetId() => AdresseId;
}