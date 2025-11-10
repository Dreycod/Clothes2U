using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;

[Table("t_e_signalement_sig")]
public class SignalementAvis : Signalement
{
    [Column("sig_avis_id")]
    public int AvisId { get; set; }
    
    //relation avec la table avis :
    
    [ForeignKey(nameof(AvisId))]
    [InverseProperty(nameof(NoteUtilisateur.Signalements))]
    public virtual NoteUtilisateur Avis { get; set; } = null!;
}