using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_signalement_sig")]
public class SignalementUtilisateur : Signalement
{
    [Column("sig_utilisateur_signale_id")]
    public int UtilisateurSignaleId { get; set; }
    
    //relation avec la table Utilisateur : 
    
    [ForeignKey(nameof(UtilisateurSignaleId))]
    [InverseProperty(nameof(Utilisateur.SignalementsUtilisateurs))]
    public virtual Utilisateur UtilisateurSignale { get; set; } = null!;
}