using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_signalement_sig")]
public class Signalement
{
    [Key]
    [Column("sig_id")]
    public int SignalementId { get; set; }
    
    [Column("sig_date")]
    public DateTime SignalementDate { get; set; }
    
    [Column("sig_motif")]
    public string SignalementMotif { get; set; } = null!;
    
    //id des relations : 
    [Column("sig_type_id")]
    public int SignalementTypeId { get; set; }
    
    [Column("sig_utilisateur_id")]
    public int UtilisateurId { get; set; }
    
    //relations avec les autres tables
    [ForeignKey(nameof(SignalementTypeId))]
    [InverseProperty(nameof(TypeSignalement.Signalements))]
    public virtual TypeSignalement TypeSignalement { get; set; } = null!;
    
    [ForeignKey(nameof(UtilisateurId))]
    [InverseProperty(nameof(Utilisateur.Signalements))]
    public virtual Utilisateur Utilisateur { get; set; } = null!;

    
}