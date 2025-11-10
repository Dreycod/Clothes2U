using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_j_abonnement_abo")]
public class Abonnement
{
    [Key]
    [Column("abo_id")]
    public int AbonnementId { get; set; }
    
    //id des relations entre les tables
    
    [Column("abo_utilisateur_suiveur")]
    public int UtilisateurSuiveurId { get; set; }
    
    [Column("abo_utilisateur_suivis")]
    public int UtilisateurSuivisId { get; set; }
    
    //relation avec les autres tables : 
    
    [ForeignKey(nameof(UtilisateurSuiveurId))]
    [InverseProperty(nameof(Utilisateur.Abonnements))]
    public virtual Utilisateur UtilisateurSuiveur { get; set; } = null!;
    
    [ForeignKey(nameof(UtilisateurSuivisId))]
    [InverseProperty(nameof(Utilisateur.Abonnes))]
    public virtual Utilisateur UtilisateurSuivis { get; set; } = null!;
}