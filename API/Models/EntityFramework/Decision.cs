using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_decision_dec")]
public class Decision : IEntity
{
    [Key]
    [Column("dec_id")]
    public int DecisionId { get; set; }
    
    [Column("dec_moderateur_id")]
    public int ModerateurId { get; set; }
    
    [Column("dec_utilisateur_id")]
    public int UtilisateurId { get; set; }
    
    [Column("dec_date_decision")]
    public DateTime DecisionDate { get; set; }
    
    
    //relation avec les autres tables : 
    [ForeignKey(nameof(ModerateurId))]
    [InverseProperty(nameof(Utilisateur.DecisionsModerateur))]
    public virtual Utilisateur Moderateur { get; set; } = null!;
    
    [ForeignKey(nameof(UtilisateurId))]
    [InverseProperty(nameof(Utilisateur.DecisionsUtilisateurSanctionne))]
    public virtual Utilisateur Utilisateur { get; set; } = null!;
    
    //relation avec la table decision avertissement: 
    [InverseProperty(nameof(DecisionAvertissement.Decision))]
    public virtual DecisionAvertissement? DecisionAvertissement { get; set; } 
    //relation avec la table decision sanction
    [InverseProperty(nameof(DecisionSanction.Decision))]
    public virtual DecisionSanction? DecisionSanction { get; set; } 
    
    public int GetId() => DecisionId;
}