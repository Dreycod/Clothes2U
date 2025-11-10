using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_decision_suspension_sus")]
public class Decision_suspension
{
    [Key]
    [Column("sus_id")]
    public int Decision_suspensionId { get; set; }
    
    [Column("sus_date_debut_suspension")]
    public DataType DateDebutSuspension { get; set; }
    
    [Column("sus_date_fin_suspension")]
    public DataType DateFinSuspension { get; set; }
    
    [Column("sus_motif_suspension")]
    public string MotifSuspension { get; set; }
    
    //relation avec la table utilisateur
    [Column("sus_utilisateur_id")]
    public int? UtilisateurId { get; set; }
    
    [Column("sus_utilisateur_admin_id")]
    public int? UtilisateurAdminId { get; set; }
    
    [Column("sus_annonce_id")]
    public int? AnnonceId { get; set; }
    
    [Column("sus_type_id")]
    public string TypeSuspensionId { get; set; }
    
    //relation avec les autres tables
    [ForeignKey(nameof(UtilisateurId))]
    [InverseProperty(nameof(Utilisateur.LesSuspensions))]
    public virtual Utilisateur UtilisateurSuspendu { get; set; }
    
    [ForeignKey(nameof(UtilisateurAdminId))]
    [InverseProperty(nameof(Utilisateur.LesDecisions))]
    public virtual Utilisateur Decisionnaire {get; set;}
    
    [ForeignKey(nameof(AnnonceId))]
    [InverseProperty(nameof(Annonce.Decisions))]
    public virtual Utilisateur AnnonceSuspendu {get; set;}
    
    [ForeignKey(nameof(TypeSuspensionId))]
    [InverseProperty(nameof(TypeSuspension.Decision_suspensions))]
    public virtual TypeSuspension TypeSuspension {get; set;}
    
}