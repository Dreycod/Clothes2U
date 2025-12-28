using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_e_demande_restauration_demres")]
public class DemandeRestauration : IEntity
{
    [Key]
    [Column("demres_id")]
    public int DemandeRestaurationId { get; set; }
    
    [Column("demres_message")]
    public string Message { get; set; }
    
    //relation avec les autres tables  :
    
    [Column("demres_decision_id")]
    public int DecisionId { get; set; }
    
    [ForeignKey(nameof(DecisionId))]
    [InverseProperty(nameof(Decision.DemandeRestauration))]
    public virtual Decision Decision { get; set; } = null!;
    
    public int GetId() => DecisionId;
}