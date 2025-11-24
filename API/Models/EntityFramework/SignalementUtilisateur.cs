using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_signalement_utilisateur_siguti")]
public class SignalementUtilisateur : IEntity
{
    [Key]
    [Column("siguti_id")]
    public int SignalementUtilisateurId { get; set; }

    [Column("siguti_utilisateur_signale_id")]
    public int UtilisateurSignaleId { get; set; }

    [Column("siguti_signalement_id")]
    public int SignalementId { get; set; }

    //relation avec la table Utilisateur : 

    [ForeignKey(nameof(UtilisateurSignaleId))]
    [InverseProperty(nameof(Utilisateur.SignalementsUtilisateurs))]
    public virtual Utilisateur UtilisateurSignale { get; set; } = null!;

    [ForeignKey(nameof(SignalementId))]
    [InverseProperty(nameof(Signalement.SignalementsUtilisateur))]
    public virtual Signalement Signalement { get; set; } = null!;

    public int GetId() => SignalementUtilisateurId;

}