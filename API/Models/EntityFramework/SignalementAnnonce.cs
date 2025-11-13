using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_signalement_sig")]
public class SignalementAnnonce : Signalement
{
  [Column("sig_annonce_signalee_id")]
  public int AnnonceSignaleeId { get; set; }
  
  //relation avec la table annonce : 
  
  
  [ForeignKey(nameof(AnnonceSignaleeId))]
  [InverseProperty(nameof(Annonce.Signalements))]
  public virtual Annonce Annonce { get; set; } = null!;
}