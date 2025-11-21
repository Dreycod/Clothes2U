using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_signalement_annonce_sigan")]
public class SignalementAnnonce : IEntity
{
    [Key]
    [Column("sigan_id")]
    public int SignalementAnnonceId { get; set; }


    [Column("sigan_annonce_signalee_id")]
    public int AnnonceSignaleeId { get; set; }

    [Column("sigan_signalement_id")]
    public int SignalementId { get; set; }

    //relation avec la table annonce : 


    [ForeignKey(nameof(AnnonceSignaleeId))]
    [InverseProperty(nameof(Annonce.Signalements))]
    public virtual Annonce Annonce { get; set; } = null!;

    [ForeignKey(nameof(SignalementId))]
    [InverseProperty(nameof(Signalement.SignalementsAnnonce))]
    public virtual Signalement Signalement { get; set; } = null!;

    public int GetId() => SignalementAnnonceId;

}