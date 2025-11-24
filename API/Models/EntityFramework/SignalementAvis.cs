using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_signalement_avis_sigavs")]
public class SignalementAvis : IEntity
{
    [Key]
    [Column("sigavs_id")]
    public int SignalementAvisId { get; set; }

    [Column("sigavs_avis_id")]
    public int AvisId { get; set; }

    [Column("sigavs_signalement_id")]
    public int SignalementId { get; set; }

    //relation avec la table avis :

    [ForeignKey(nameof(AvisId))]
    [InverseProperty(nameof(NoteUtilisateur.Signalements))]
    public virtual NoteUtilisateur Avis { get; set; } = null!;

    [ForeignKey(nameof(SignalementId))]
    [InverseProperty(nameof(Signalement.SignalementsAvis))]
    public virtual Signalement Signalement { get; set; } = null!;

    public int GetId() => SignalementAvisId;

}