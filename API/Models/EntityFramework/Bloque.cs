using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_bloque_blo")]
public class Bloque
{
    [Key]
    [Column("blo_id")]
    public int BloqueId { get; set; }
    
    //id des relations
    [Column("blo_utilisateur_bloqueur_id")]
    public int UtilisateurBloqueurId { get; set; }
    
    [Column("blo_utilisateur_bloque_id")]
    public int UtilisateurBloqueId { get; set; }
    
    //relation avec les autres tables
    
    [ForeignKey(nameof(UtilisateurBloqueurId))]
    [InverseProperty(nameof(Utilisateur.UtilisateursBloques))]
    public virtual Utilisateur UtilisateurBloqueur { get; set; } = null!;
    
    [ForeignKey(nameof(UtilisateurBloqueId))]
    [InverseProperty(nameof(Utilisateur.BloqueParUtilisateurs))]
    public virtual Utilisateur UtilisateurBloque { get; set; } = null!;
}