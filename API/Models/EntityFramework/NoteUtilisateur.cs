using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models.EntityFramework;


[Table("t_e_note_utilisateur_notuti")]
public class NoteUtilisateur : IEntity
{
    [Key]
    [Column("notuti_id")]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
    public int NoteUtilisateurId { get; set; }

    [Column("notuti_note")]
    [Range(0, 5)]
    public int Note { get; set; }

    [Column("notuti_commentaire")]
    [MaxLength(200)]
    public string Commentaire { get; set; }

    [Column("notuti_date")]
    public DateTime DatePublication { get; set; }

    [Column("notuti_statut")]
    public bool Statut { get; set; }

    // id pour les relations
    [Column("notuti_noteur_id")]
    public int AuteurId { get; set; }

    [Column("notuti_cible_id")]
    public int CibleId { get; set; }

    [ForeignKey(nameof(AuteurId))]
    [InverseProperty(nameof(Utilisateur.NotesAuteur))]
    public Utilisateur Auteur { get; set; }

    [ForeignKey(nameof(CibleId))]
    [InverseProperty(nameof(Utilisateur.NotesCible))]
    public Utilisateur Cible { get; set; }

    [InverseProperty(nameof(SignalementAvis.Avis))]
    public ICollection<SignalementAvis> Signalements { get; set; } = new List<SignalementAvis>();

    [InverseProperty(nameof(Decision_suspension.Avis))]
    public ICollection<Decision_suspension> Decision_Suspensions { get; set; } = new List<Decision_suspension>();

    [InverseProperty(nameof(ElementDecisionAvis.Avis))]
    public ICollection<ElementDecisionAvis> Elementsdecisionavis { get; set; } = new List<ElementDecisionAvis>();

    public int GetId() => NoteUtilisateurId;
}
