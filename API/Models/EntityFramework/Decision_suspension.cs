using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_decision_suspension_sus")]
public class Decision_suspension : IEntity
{
    [Key]
    [Column("sus_id")]
    public int Decision_suspensionId { get; set; }
    
    [Column("sus_date_debut_suspension")]
    public DateTime DateDebutSuspension { get; set; }
    
    
    [Column("sus_motif_suspension")]
    public string? MotifSuspension { get; set; }

    [Column("sus_traitee")]
    public bool EstTraitee { get; set; } = false;
    
    [Column("sus_utilisateur_id")]
    public int? UtilisateurId { get; set; }
    
    [Column("sus_utilisateur_admin_id")]
    public int? UtilisateurAdminId { get; set; }
    
    [Column("sus_annonce_id")]
    public int? AnnonceId { get; set; }

    [Column("sus_avis_id")]
    public int? AvisId { get; set; }

    [Column("sus_message_id")]
    public int? MessageId { get; set; }

    [Column("sus_type_id")]
    public int TypeSuspensionId { get; set; }
    
    [ForeignKey(nameof(UtilisateurId))]
    [InverseProperty(nameof(Utilisateur.LesSuspensions))]
    public virtual Utilisateur? UtilisateurSuspendu { get; set; }
    
    [ForeignKey(nameof(UtilisateurAdminId))]
    [InverseProperty(nameof(Utilisateur.LesDecisions))]
    public virtual Utilisateur? Decisionnaire { get; set; }
    
    [ForeignKey(nameof(AnnonceId))]
    [InverseProperty(nameof(Annonce.Decisions))]
    public virtual Annonce? AnnonceSuspendu { get; set; }
    
    [ForeignKey(nameof(TypeSuspensionId))]
    [InverseProperty(nameof(TypeSuspension.Decision_suspensions))]
    public virtual TypeSuspension TypeSuspension { get; set; }

    [ForeignKey(nameof(AvisId))]
    [InverseProperty(nameof(NoteUtilisateur.Decision_Suspensions))]
    public virtual NoteUtilisateur Avis { get; set; }

    [ForeignKey(nameof(MessageId))]
    [InverseProperty(nameof(Message.Decision_suspensions))]
    public virtual Message MessageDecision { get; set; }

    [InverseProperty(nameof(DemandeRestauration.Suspension))]
    public virtual ICollection<DemandeRestauration> DemandesRes { get; set; } = new List<DemandeRestauration>();

    [InverseProperty(nameof(Sanction.Decision_sus))]
    public virtual ICollection<Sanction> Sanctions { get; set; } = new List<Sanction>();

    [InverseProperty(nameof(DecisionAvertissement.DecisionSuspension))]
    public virtual ICollection<DecisionAvertissement> DecisionAvertissements { get; set; } = new List<DecisionAvertissement>();

    public int GetId() => Decision_suspensionId;

}